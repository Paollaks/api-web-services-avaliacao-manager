using System;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;

public class TmdbService
{
    private readonly HttpClient _httpClient;
    private const string ApiKey = "SUA_CHAVE_DE_API"; // Substitua pela sua chave de API
    private const string BaseUrl = "https://api.themoviedb.org/3";

    public TmdbService(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    public async Task<string> GetTopRatedMoviesAsync(int page = 1)
    {
        try
        {
            var url = $"{BaseUrl}/movie/top_rated?api_key={ApiKey}&language=pt-BR&page={page}";
            var response = await _httpClient.GetFromJsonAsync<TmdbResponse>(url);

            if (response != null && response.Results != null)
            {
                foreach (var movie in response.Results)
                {
                    Console.WriteLine($"Título: {movie.Title}, Nota: {movie.VoteAverage}");
                }
            }

            return "Busca concluída com sucesso!";
        }
        catch (Exception ex)
        {
            return $"Erro ao buscar filmes: {ex.Message}";
        }
    }
}

public class TmdbResponse
{
    public int Page { get; set; }
    public Movie[] Results { get; set; }
}

public class Movie
{
    public string Title { get; set; }
    public double VoteAverage { get; set; }
}
