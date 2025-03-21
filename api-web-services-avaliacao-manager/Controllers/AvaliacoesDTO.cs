using Microsoft.AspNetCore.Mvc;
using System.Net.Http;
using System.Threading.Tasks;
using System.Collections.Generic;
using api_web_services_avaliacao_manager.Models;
using System.Text.Json;
using System;

namespace api_web_services_avaliacao_manager.Controllers
{
    [ApiController]
    [Route("api/avaliacoes")]
    public class AvaliacoesController : ControllerBase
    {
        private readonly HttpClient _httpClient;
        private readonly string _apiKey = "ad4825b13d87693db59396001bcd68f5"; // 🔹 Substitua pela sua API Key do TMDB

        public AvaliacoesController(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        [HttpGet("{filmeId}")]
        public async Task<IActionResult> GetAvaliacoes(int filmeId)
        {
            try
            {
                // 🔹 Buscar título do filme
                string movieUrl = $"https://api.themoviedb.org/3/movie/{filmeId}?api_key={_apiKey}&language=pt-BR";
                HttpResponseMessage movieResponse = await _httpClient.GetAsync(movieUrl);

                if (!movieResponse.IsSuccessStatusCode)
                {
                    return StatusCode((int)movieResponse.StatusCode, "Erro ao buscar detalhes do filme.");
                }

                string movieJsonResponse = await movieResponse.Content.ReadAsStringAsync();
                var movieData = JsonSerializer.Deserialize<MovieData>(movieJsonResponse, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

                if (movieData == null || string.IsNullOrEmpty(movieData.Title))
                {
                    return StatusCode(500, "Erro ao processar os detalhes do filme.");
                }

                // 🔹 Buscar avaliações do filme
                string url = $"https://api.themoviedb.org/3/movie/{filmeId}/reviews?api_key={_apiKey}&language=pt-BR";
                HttpResponseMessage response = await _httpClient.GetAsync(url);

                if (!response.IsSuccessStatusCode)
                {
                    return StatusCode((int)response.StatusCode, "Erro ao buscar avaliações.");
                }

                string jsonResponse = await response.Content.ReadAsStringAsync();
                var tmdbResponse = JsonSerializer.Deserialize<TmdbReviewResponse>(jsonResponse, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

                if (tmdbResponse?.results == null || tmdbResponse.results.Count == 0)
                {
                    return NotFound("Nenhuma avaliação encontrada para este filme.");
                }

                // 🔹 Mapeia as avaliações do TMDB para o DTO esperado
                var avaliacoes = new List<AvaliacaoDTO>();
                foreach (var review in tmdbResponse.results)
                {
                    avaliacoes.Add(new AvaliacaoDTO
                    {
                        FilmeId = filmeId,
                        TituloFilme = movieData.Title, // 🔹 Agora incluímos o título
                        Usuario = review.author_details?.username ?? "Usuário desconhecido",
                        Nota = review.author_details?.rating ?? 0, // Se não houver nota, assume 0
                        Comentario = review.content ?? "Sem comentário."
                    });
                }

                return Ok(avaliacoes);
            }
            catch (HttpRequestException ex)
            {
                return StatusCode(500, $"Erro ao acessar API externa: {ex.Message}");
            }
            catch (JsonException ex)
            {
                return StatusCode(500, $"Erro ao processar resposta JSON: {ex.Message}");
            }
        }
    }

    // 🔹 Modelo para mapear os detalhes do filme
    public class MovieData
    {
        public string Title { get; set; }
    }

    // 🔹 Modelo para mapear a resposta da API do TMDB
    public class TmdbReviewResponse
    {
        public List<Review> results { get; set; } = new();
    }

    public class Review
    {
        public AuthorDetails author_details { get; set; }
        public string content { get; set; }
    }

    public class AuthorDetails
    {
        public string username { get; set; }
        public double? rating { get; set; }
    }

    public class AvaliacaoDTO
    {
        public int FilmeId { get; set; }
        public string TituloFilme { get; set; } // 🔹 Adicionado para armazenar o título
        public string Usuario { get; set; }
        public double Nota { get; set; }
        public string Comentario { get; set; }
    }
}
