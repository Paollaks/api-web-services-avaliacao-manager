using System.Text.Json;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Net.Http;
using System.Threading.Tasks;
using System.Collections.Generic;
using api_web_services_avaliacao_manager.Models;
using System;
using Microsoft.AspNetCore.Authorization;


namespace api_web_services_avaliacao_manager.Controllers
{
   
    [ApiController]
    [Route("api/avaliacoes")]
    public class AvaliacoesController : ControllerBase
    {
        private readonly HttpClient _httpClient;
        private readonly string _apiKey = "ad4825b13d87693db59396001bcd68f5";

        public AvaliacoesController(HttpClient httpClient) => _httpClient = httpClient;

       
        [HttpGet("top-rated")]
        public async Task<IActionResult> GetTopRatedMovies()
        {
            try
            {
                // 🔹 URL para buscar os filmes com as maiores notas
                string url = $"https://api.themoviedb.org/3/movie/top_rated?api_key={_apiKey}&language=pt-BR";

                HttpResponseMessage response = await _httpClient.GetAsync(url);

                if (!response.IsSuccessStatusCode)
                {
                    return StatusCode((int)response.StatusCode, "Erro ao buscar filmes com as maiores notas.");
                }

                string jsonResponse = await response.Content.ReadAsStringAsync();
                var tmdbResponse = JsonSerializer.Deserialize<TmdbTopRatedResponse>(jsonResponse, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

                if (tmdbResponse?.results == null || tmdbResponse.results.Count == 0)
                {
                    return NotFound("Nenhum filme encontrado com as maiores notas.");
                }

                // 🔹 Mapeia os filmes para o DTO esperado
                var filmes = new List<FilmeTopRatedDTO>();
                foreach (var movie in tmdbResponse.results)
                {
                    filmes.Add(new FilmeTopRatedDTO
                    {
                        FilmeId = movie.id,
                        Titulo = movie.title,
                        NotaMedia = movie.vote_average,
                        Resumo = movie.overview ?? "Sem resumo disponível."
                    });
                }

                return Ok(filmes);
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

        // 🔹 Modelo para mapear a resposta da API do TMDB
        public class TmdbTopRatedResponse
        {
            public List<Movie> results { get; set; } = new();
        }

        public class Movie
        {
            public int id { get; set; }
            public string title { get; set; }
            public double vote_average { get; set; }
            public string overview { get; set; }
        }

        // 🔹 DTO para retornar os filmes com as maiores notas
        public class FilmeTopRatedDTO
        {
            public int FilmeId { get; set; }
            public string Titulo { get; set; }
            public double NotaMedia { get; set; }
            public string Resumo { get; set; }
        }
    }
}