using api_web_services_avaliacao_manager.Models;
using api_web_services_avaliacao_manager.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace api_web_services_avaliacao_manager.Controllers
{

    [Route("api/Comentarios")]
    [ApiController]
    public class ComentariosController : ControllerBase
    {
        private readonly AppDbContext _context;
        private readonly TMDBService _tmdbService;

        public ComentariosController(AppDbContext context, TMDBService tmdbService)
        {
            _context = context;
            _tmdbService = tmdbService;
        }

        [HttpGet]
        public async Task<ActionResult> GetAll()
        {
            var comentarios = await _context.Comentarios.ToListAsync();

            // Filtrar apenas os comentários relacionados a filmes válidos
            var comentariosValidos = new List<Comentario>();
            foreach (var comentario in comentarios)
            {
                if (await IsFilmeValido(comentario.TMDBFilmeId))
                {
                    comentariosValidos.Add(comentario);
                }
            }

            return Ok(comentariosValidos);
        }

        [HttpPost]
        public async Task<ActionResult> Create(Comentario model)
        {
            // Validar se o filme é válido
            if (!await IsFilmeValido(model.TMDBFilmeId))
            {
                return NotFound($"O comentário não está relacionado a um filme válido.");
            }

            // Criar o comentário com base no modelo recebido
            var comentario = new Comentario
            {
                Texto = model.Texto,
                IdUsuario = model.IdUsuario,
                TMDBFilmeId = model.TMDBFilmeId
            };

            // Adicionar o comentário ao banco de dados
            _context.Comentarios.Add(comentario);
            await _context.SaveChangesAsync();

            // Retornar o comentário criado
            return CreatedAtAction("GetById", new { id = comentario.Id }, comentario);
        }

        [HttpGet("{Id}")]
        public async Task<ActionResult> GetById(int Id)
        {
            var model = await _context.Comentarios
                .FirstOrDefaultAsync(c => c.Id == Id);

            if (model == null) return NotFound();

            if (!await IsFilmeValido(model.TMDBFilmeId))
            {
                return BadRequest($"O comentário existe, mas o filme relacionado (ID {model.TMDBFilmeId}) não é válido.");
            }

            GerarLinks(model);
            return Ok(model);
        }
        [HttpPut("{Id}")]
        public async Task<ActionResult> Update(int Id, Comentario model)
        {
            if (Id != model.Id) return BadRequest();

            var modelDb = await _context.Comentarios.AsNoTracking()
                .FirstOrDefaultAsync(c => c.Id == Id);
            if (modelDb == null) return NotFound();

            if (!await IsFilmeValido(model.TMDBFilmeId))
            {
                return BadRequest($"O filme relacionado ao comentário (ID {model.TMDBFilmeId}) não é válido.");
            }

            _context.Comentarios.Update(model);
            await _context.SaveChangesAsync();
            return NoContent();
        }
        [HttpDelete("{Id}")]
        public async Task<ActionResult> Delete(int Id)
        {
            var model = await _context.Comentarios.FindAsync(Id);

            if (model == null) return NotFound();

            if (!await IsFilmeValido(model.TMDBFilmeId))
            {
                return BadRequest($"O filme relacionado ao comentário (ID {model.TMDBFilmeId}) não é válido.");
            }

            _context.Comentarios.Remove(model);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private void GerarLinks(Comentario model)
        {
            model.Links.Add(new LinkDTO(model.Id, Url.ActionLink(), rel: "self", metodo: "GET"));
            model.Links.Add(new LinkDTO(model.Id, Url.ActionLink(), rel: "update", metodo: "PUT"));
            model.Links.Add(new LinkDTO(model.Id, Url.ActionLink(), rel: "delete", metodo: "DELETE"));
        }
        private async Task<bool> IsFilmeValido(int tmdbFilmeId)
        {
            var filme = await _tmdbService.GetFilmeByIdAsync(tmdbFilmeId);
            return filme != null;
        }
    }
}
