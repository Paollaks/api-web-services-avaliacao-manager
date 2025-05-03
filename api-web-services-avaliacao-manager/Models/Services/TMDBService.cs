using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json.Serialization;
using Microsoft.Extensions.Configuration;
using api_web_services_avaliacao_manager.Utils;
using api_web_services_avaliacao_manager.Models;

namespace api_web_services_avaliacao_manager.Services
{
    public class TMDBService
    {
        private readonly HttpClient _httpClient;
        private readonly string _apiKey;
        private const string BaseUrl = "https://api.themoviedb.org/3";

        public TMDBService(HttpClient httpClient, IConfiguration configuration)
        {
            _httpClient = httpClient;
            _apiKey = configuration["TMDB_API_KEY"];

            if (string.IsNullOrEmpty(_apiKey))
            {
                throw new System.Exception("API Key do TMDB não configurada corretamente.");
            }
        }

        public async Task<List<Filme>> GetFilmesPopularesAsync()
        {
            var url = $"{BaseUrl}/movie/popular?api_key={_apiKey}&language=pt-BR";

            var response = await _httpClient.GetAsync(url);
            if (!response.IsSuccessStatusCode)
            {
                return new List<Filme>();
            }

            var jsonResponse = await response.Content.ReadAsStringAsync();
            var tmdbResponse = JsonSerializer.Deserialize<TMDBResponse>(jsonResponse, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

            var filmes = new List<Filme>();
            if (tmdbResponse?.Results != null)
            {
                foreach (var item in tmdbResponse.Results)
                {
                    filmes.Add(new Filme
                    {
                        Id = item.Id,
                        Titulo = item.Title,
                        AnoLancamento = int.TryParse(item.ReleaseDate?.Split('-')[0], out var ano) ? ano : 0,
                        Genero = item.Genres != null ? string.Join(", ", item.Genres.Select(g => g.Name)) : "Desconhecido",
                        Sinopse = item.Overview,
                        FotoUrl = $"https://image.tmdb.org/t/p/w500{item.PosterPath}",
                        NotaMedia = item.VoteAverage
                    });
                }
            }
            return filmes;
        }

        public async Task<List<Filme>> ObterFilmesPorGenero(int idGenero)
        {
            string url = $"{BaseUrl}/discover/movie?api_key={_apiKey}&language=pt-BR&with_genres={idGenero}";

            HttpResponseMessage response = await _httpClient.GetAsync(url);
            if (!response.IsSuccessStatusCode)
            {
                return new List<Filme>();
            }

            string json = await response.Content.ReadAsStringAsync();
            using JsonDocument doc = JsonDocument.Parse(json);
            JsonElement root = doc.RootElement.GetProperty("results");

            var filmes = new List<Filme>();
            foreach (var item in root.EnumerateArray())
            {
                filmes.Add(new Filme
                {
                    Id = item.GetProperty("id").GetInt32(),
                    Titulo = item.GetProperty("title").GetString(),
                    Genero = GenerosTMDB.Generos.TryGetValue(idGenero, out var nomeGenero) ? nomeGenero : "Desconhecido",
                    Sinopse = item.GetProperty("overview").GetString(),
                    AnoLancamento = int.TryParse(item.GetProperty("release_date").GetString()?.Split('-')[0], out var ano) ? ano : 0,
                    FotoUrl = $"https://image.tmdb.org/t/p/w500{item.GetProperty("poster_path").GetString()}",
                    NotaMedia = item.GetProperty("vote_average").GetDouble()
                });
            }

            return filmes;
        }

        public async Task<Filme> GetFilmeByIdAsync(int id)
        {
            var url = $"{BaseUrl}/movie/{id}?api_key={_apiKey}&language=pt-BR";

            var response = await _httpClient.GetAsync(url);
            if (!response.IsSuccessStatusCode)
            {
                return null;
            }

            var jsonResponse = await response.Content.ReadAsStringAsync();
            var item = JsonSerializer.Deserialize<TMDBFilme>(jsonResponse, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

            if (item == null) return null;

            int anoLancamento = 0;
            if (!string.IsNullOrWhiteSpace(item.ReleaseDate) && item.ReleaseDate.Length >=4)
            {
                int.TryParse(item.ReleaseDate.Substring(0, 4), out anoLancamento);
            }

            return new Filme
            {
                Id = item.Id,
                Titulo = item.Title,
                AnoLancamento = anoLancamento,
                Genero = item.Genres != null ? string.Join(", ", item.Genres.Select(g => g.Name)) : "Desconhecido",
                Sinopse = item.Overview,
                FotoUrl = $"https://image.tmdb.org/t/p/w500{item.PosterPath}",
                NotaMedia = item.VoteAverage

            };
        }


        public class TMDBResponse
        {
            public List<TMDBFilme> Results { get; set; }
        }

    public class TMDBFilme
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Overview { get; set; }
        public string ReleaseDate { get; set; }
        public List<TMDBGenero> Genres { get; set; }

        public string PosterPath { get; set; }
            
        [JsonPropertyName("vote_average")]
        public double VoteAverage { get; set; }
        }

        public class TMDBGenero
        {
            public int Id { get; set; }
            public string Name { get; set; }
        }
    }
}
