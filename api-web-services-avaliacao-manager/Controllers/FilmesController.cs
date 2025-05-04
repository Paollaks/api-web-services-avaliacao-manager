using api_web_services_avaliacao_manager.Models;
using api_web_services_avaliacao_manager.Services;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace api_web_services_avaliacao_manager.Controllers
{
    [Route("api/Filmes")]
    [ApiController]
    public class FilmesController : ControllerBase
    {
        private readonly TMDBService _tmdbService;

        public FilmesController(TMDBService tmdbService)
        {
            _tmdbService = tmdbService;
        }

        [HttpGet("tmdb")]
        public async Task<ActionResult<IEnumerable<Filme>>> GetFilmesPopularesTMDB([FromQuery] string? termo)
        {
            List<Filme> filmes;

            if (!string.IsNullOrWhiteSpace(termo))
            {
                filmes = await _tmdbService.BuscarFilmesPorTituloAsync(termo);
            }
            else
            {
                filmes = await _tmdbService.GetFilmesPopularesAsync();
            }

            if (filmes == null || filmes.Count == 0)
            {
                return NotFound("Nenhum filme encontrado.");
            }

            return Ok(filmes);
        }


        [HttpGet("tmdb/{id}")]
        public async Task<ActionResult<Filme>> GetFilmeById(int id)
        {
            var filme = await _tmdbService.GetFilmeByIdAsync(id);

            if (filme == null)
            {
                return NotFound($"Filme com ID {id} não encontrado.");
            }

            return Ok(filme);
        }

        [HttpGet("genero/{idGenero}")]
        public async Task<ActionResult<IEnumerable<Filme>>> GetFilmesPorGenero(int idGenero)
        {
            var filmes = await _tmdbService.ObterFilmesPorGenero(idGenero);

            if (filmes == null || filmes.Count == 0)
            {
                return NotFound($"Nenhum filme encontrado para o gênero {idGenero}.");
            }

            return Ok(filmes);
        }
    }
}
