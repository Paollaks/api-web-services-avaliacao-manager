﻿using api_web_services_avaliacao_manager.Models;
using api_web_services_avaliacao_manager.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace api_web_services_avaliacao_manager.Controllers
{
    //comentario teste 
    [Authorize]
    [Route("api/Favoritos")]
    [ApiController]
    public class FavoritosController : ControllerBase
    {
        private readonly AppDbContext _context;
        private readonly TMDBService _tmdbService;

        public FavoritosController(AppDbContext context, TMDBService tmdbService)
        {
            _context = context;
            _tmdbService = tmdbService;
        }

        // POST: api/favoritos
        [HttpPost]
        public async Task<ActionResult> AddFavorito(Favorito favorito)
        {
            // Verifica se o usuário existe
            var usuario = await _context.Usuarios.FindAsync(favorito.IdUsuario);
            if (usuario == null)
            {
                return NotFound("Usuário não encontrado.");
            }

            // Verifica se o filme existe no banco de dados
            var filme = await _context.Filmes.FindAsync(favorito.IdFilme);
            if (filme == null)
            {
                // Busca o filme na API do TMDB e adiciona ao banco de dados
                filme = await _tmdbService.GetFilmeByIdAsync(favorito.IdFilme);
                if (filme == null)
                {
                    return NotFound("Filme não encontrado.");
                }

                _context.Filmes.Add(filme);
                await _context.SaveChangesAsync();
            }

            // Verifica se o favorito já existe
            var favoritoExistente = await _context.Favoritos
                .FirstOrDefaultAsync(f => f.IdUsuario == favorito.IdUsuario && f.IdFilme == favorito.IdFilme);
            if (favoritoExistente != null)
            {
                return BadRequest("O filme já está nos favoritos.");
            }

            // Adiciona o filme aos favoritos
            _context.Favoritos.Add(favorito);
            await _context.SaveChangesAsync();

            return Ok("Filme adicionado aos favoritos!");
        }

        // DELETE: api/favoritos
        [HttpDelete("{idUsuario}/{idFilme}")]
        public async Task<ActionResult> DeleteFavorito(int idUsuario, int idFilme)
        {
            // Verifica se o favorito existe
            var favorito = await _context.Favoritos
                .FirstOrDefaultAsync(f => f.IdUsuario == idUsuario && f.IdFilme == idFilme);
            if (favorito == null)
            {
                return NotFound("Favorito não encontrado.");
            }

            // Remove o filme dos favoritos
            _context.Favoritos.Remove(favorito);
            await _context.SaveChangesAsync();

            return Ok("Filme removido dos favoritos!");
        }

        [HttpGet]
        public async Task<ActionResult> GetAll()
        {
            // Obtém todos os favoritos com os detalhes do filme
            var favoritos = await _context.Favoritos
                .Include(f => f.Filme)
                .Select(f => new
                {
                    f.IdUsuario,
                    f.IdFilme,
                    Filme = new
                    {
                        f.Filme.Id,
                        f.Filme.Titulo
                    }
                })
                .ToListAsync();

            return Ok(favoritos);
        }

        [HttpGet("usuario/{idUsuario}/filmes")]
        public async Task<ActionResult> GetFilmesPorUsuario(int idUsuario)
        {
            // Buscar os comentários do usuário no banco de dados
            var favoritos = await _context.Favoritos
                .Where(c => c.IdUsuario == idUsuario)
                .ToListAsync();

            if (!favoritos.Any())
            {
                return NotFound($"Nenhum filme favoritado encontrado para o usuário com ID {idUsuario}.");
            }

            // Obter os filmes relacionados aos comentários
            var filmes = new List<Filme>();
            foreach (var favorito in favoritos)
            {
                var filme = await _tmdbService.GetFilmeByIdAsync(favorito.IdFilme);
                if (filme != null && !filmes.Any(f => f.Id == filme.Id)) // Evitar duplicatas
                {
                    filmes.Add(filme);
                }
            }

            if (!filmes.Any())
            {
                return NotFound($"Nenhum filme relacionado encontrado para o usuário com ID {idUsuario}.");
            }

            return Ok(filmes);
        }
    }
}