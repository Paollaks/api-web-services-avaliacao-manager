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
            var filmes = new List<Filme>();

            for (int page = 1; page <= 3; page++) // 👈 busca 3 páginas (60 filmes)
            {
                var url = $"{BaseUrl}/movie/popular?api_key={_apiKey}&language=pt-BR&page={page}";

                var response = await _httpClient.GetAsync(url);
                if (!response.IsSuccessStatusCode)
                    continue;

                var jsonResponse = await response.Content.ReadAsStringAsync();
                using JsonDocument doc = JsonDocument.Parse(jsonResponse);
                var results = doc.RootElement.GetProperty("results");

                foreach (var item in results.EnumerateArray())
                {
                    var id = item.GetProperty("id").GetInt32();
                    var titulo = item.GetProperty("title").GetString();
                    var sinopse = item.GetProperty("overview").GetString();
                    var releaseDate = item.GetProperty("release_date").GetString();
                    var nota = item.GetProperty("vote_average").GetDouble();
                    var posterPath = item.GetProperty("poster_path").GetString();
                    var genreIds = item.GetProperty("genre_ids").EnumerateArray().Select(g => g.GetInt32()).ToList();

                    var genero = genreIds.Count > 0 && GenerosTMDB.Generos.TryGetValue(genreIds[0], out var nomeGenero)
                        ? nomeGenero
                        : "Desconhecido";

                    var ano = int.TryParse(releaseDate?.Split('-')[0], out var anoExtraido) ? anoExtraido : 0;

                    var fotoUrl = !string.IsNullOrEmpty(posterPath)
                        ? $"https://image.tmdb.org/t/p/w500{posterPath}"
                        : null;

                    filmes.Add(new Filme
                    {
                        Id = id,
                        Titulo = titulo,
                        AnoLancamento = ano,
                        Genero = genero,
                        Sinopse = sinopse,
                        FotoUrl = fotoUrl,
                        NotaMedia = nota
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
            Console.WriteLine($"Resposta da API: {jsonResponse}"); // Log da resposta completa

            var item = JsonSerializer.Deserialize<TMDBFilme>(jsonResponse, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });


            if (item == null) return null;

            int anoLancamento = 0;
            if (!string.IsNullOrWhiteSpace(item.ReleaseDate) && item.ReleaseDate.Length >=4)
            {
                int.TryParse(item.ReleaseDate.Substring(0, 4), out anoLancamento);
            }

            var posterPath = item.poster_path;
            Console.WriteLine($"PosterPath retornado pela API: {posterPath}");

            var fotoUrl = !string.IsNullOrEmpty(posterPath)
                ? $"https://image.tmdb.org/t/p/w500{posterPath}"
                : null;


            return new Filme
            {
                Id = item.Id,
                Titulo = item.Title,
                AnoLancamento = anoLancamento,
                Genero = item.Genres != null ? string.Join(", ", item.Genres.Select(g => g.Name)) : "Desconhecido",
                Sinopse = item.Overview,
                FotoUrl = $"https://image.tmdb.org/t/p/w500{posterPath}",
                NotaMedia = item.VoteAverage

            };
        }
        public async Task<List<Filme>> BuscarFilmesPorTituloAsync(string termo)
        {
            var url = $"{BaseUrl}/search/movie?api_key={_apiKey}&language=pt-BR&query={Uri.EscapeDataString(termo)}";

            var response = await _httpClient.GetAsync(url);
            if (!response.IsSuccessStatusCode)
                return new List<Filme>();

            var jsonResponse = await response.Content.ReadAsStringAsync();
            using JsonDocument doc = JsonDocument.Parse(jsonResponse);
            var root = doc.RootElement.GetProperty("results");

            var filmes = new List<Filme>();
            foreach (var item in root.EnumerateArray())
            {
                var id = item.GetProperty("id").GetInt32();
                var titulo = item.GetProperty("title").GetString();
                var sinopse = item.GetProperty("overview").GetString();
                var releaseDate = item.GetProperty("release_date").GetString();
                var nota = item.GetProperty("vote_average").GetDouble();
                var posterPath = item.GetProperty("poster_path").GetString();

                // Gênero
                var generoIds = item.TryGetProperty("genre_ids", out var generosElement) && generosElement.ValueKind == JsonValueKind.Array
                    ? generosElement.EnumerateArray().Select(g => g.GetInt32())
                    : Enumerable.Empty<int>();

                var nomesGeneros = generoIds
                    .Where(idGenero => GenerosTMDB.Generos.ContainsKey(idGenero))
                    .Select(idGenero => GenerosTMDB.Generos[idGenero]);

                var genero = nomesGeneros.Any() ? string.Join(", ", nomesGeneros) : "Desconhecido";

                var ano = int.TryParse(releaseDate?.Split('-')[0], out var anoExtraido) ? anoExtraido : 0;
                var fotoUrl = !string.IsNullOrEmpty(posterPath)
                    ? $"https://image.tmdb.org/t/p/w500{posterPath}"
                    : null;

                filmes.Add(new Filme
                {
                    Id = id,
                    Titulo = titulo,
                    AnoLancamento = ano,
                    Genero = genero,
                    Sinopse = sinopse,
                    FotoUrl = fotoUrl,
                    NotaMedia = nota
                });
            }

            return filmes;
        }


        public async Task<List<Filme>> GetFilmesTopRatedAsync()
        {
            var filmes = new List<Filme>();

            for (int page = 1; page <= 3; page++) // Busca 3 páginas (60 filmes)
            {
                var url = $"{BaseUrl}/movie/top_rated?api_key={_apiKey}&language=pt-BR&page={page}";

                var response = await _httpClient.GetAsync(url);
                if (!response.IsSuccessStatusCode)
                    continue;

                var jsonResponse = await response.Content.ReadAsStringAsync();
                using JsonDocument doc = JsonDocument.Parse(jsonResponse);
                var results = doc.RootElement.GetProperty("results");

                foreach (var item in results.EnumerateArray())
                {
                    var id = item.GetProperty("id").GetInt32();
                    var titulo = item.GetProperty("title").GetString();
                    var sinopse = item.GetProperty("overview").GetString();
                    var releaseDate = item.GetProperty("release_date").GetString();
                    var nota = item.GetProperty("vote_average").GetDouble();
                    var posterPath = item.GetProperty("poster_path").GetString();
                    var genreIds = item.GetProperty("genre_ids").EnumerateArray().Select(g => g.GetInt32()).ToList();

                    var genero = genreIds.Count > 0 && GenerosTMDB.Generos.TryGetValue(genreIds[0], out var nomeGenero)
                        ? nomeGenero
                        : "Desconhecido";

                    var ano = int.TryParse(releaseDate?.Split('-')[0], out var anoExtraido) ? anoExtraido : 0;

                    var fotoUrl = !string.IsNullOrEmpty(posterPath)
                        ? $"https://image.tmdb.org/t/p/w500{posterPath}"
                        : null;

                    filmes.Add(new Filme
                    {
                        Id = id,
                        Titulo = titulo,
                        AnoLancamento = ano,
                        Genero = genero,
                        Sinopse = sinopse,
                        FotoUrl = fotoUrl,
                        NotaMedia = nota
                    });
                }
            }

            return filmes;
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

        public string poster_path { get; set; }
            
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
