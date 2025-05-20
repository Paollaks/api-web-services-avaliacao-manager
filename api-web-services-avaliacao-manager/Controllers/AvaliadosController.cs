using api_web_services_avaliacao_manager.Models;
using api_web_services_avaliacao_manager.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;


namespace api_web_services_avaliacao_manager.Controllers
{
    [ApiController]
    [Route("api/Avaliacoes")]
    public class AvaliacoesController : ControllerBase
    {
        private readonly AppDbContext _context;
        private readonly TMDBService _tmdbService;

        public AvaliacoesController(AppDbContext context, TMDBService tmdbService)
        {
            _context = context;
            _tmdbService = tmdbService;
        }

        [AllowAnonymous]
        [HttpGet("top-rated")]
        public async Task<IActionResult> GetTopRatedMovies()
        {
            try
            {
                var filmes = await _tmdbService.GetFilmesTopRatedAsync();
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
    }
}