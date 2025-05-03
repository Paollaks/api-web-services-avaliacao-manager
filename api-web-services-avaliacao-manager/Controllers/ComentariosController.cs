using api_web_services_avaliacao_manager.Models;
using api_web_services_avaliacao_manager.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace api_web_services_avaliacao_manager.Controllers
{
<<<<<<< HEAD
    [Route("api/comentarios")]
=======
    [Route("api/Comentarios")]
>>>>>>> 764877b3bda7485d901b4859b367b84258bae895
    [ApiController]
    public class ComentariosController(AppDbContext context, TMDBService tmdbService) : ControllerBase
    {
        private readonly AppDbContext _context = context;
        private readonly TMDBService _tmdbService = tmdbService;

        [HttpGet]
        public async Task<ActionResult> GetAll()
        {
            var model = await _context.Comentarios.ToListAsync();
            return Ok(model);
        }

        [HttpPost]
        public async Task<ActionResult> Create(Comentario model)
        {
            _context.Comentarios.Add(model);

            await _context.SaveChangesAsync();

            return CreatedAtAction("GetById", new { id = model.Id }, model);
        }

        [HttpGet("{Id}")]
        public async Task<ActionResult> GetById(int Id)
        {
            var model = await _context.Comentarios
            .FirstOrDefaultAsync(c => c.Id == Id);
            if (model == null) return NotFound();

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
            _context.Comentarios.Update(model);
            await _context.SaveChangesAsync();
            return NoContent();

        }
        [HttpDelete("{Id}")]
        public async Task<ActionResult> Delete(int Id)
        {
            var model = await _context.Comentarios.FindAsync(Id);

            if (model == null) return NotFound();

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
    }
}
