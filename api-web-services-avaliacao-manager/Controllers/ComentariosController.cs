using api_web_services_avaliacao_manager.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace api_web_services_avaliacao_manager.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ComentariosController : ControllerBase
    {
        private readonly AppDbContext _context;
        public ComentariosController(AppDbContext context)
        {
            _context = context;
        }
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
