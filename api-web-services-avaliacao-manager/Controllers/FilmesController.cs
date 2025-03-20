using api_web_services_avaliacao_manager.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace api_web_services_avaliacao_manager.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class FilmesController : ControllerBase
    {
        private readonly AppDbContext _context;
        public FilmesController(AppDbContext context) 
        {
            _context = context; 
        }
        [HttpGet]
        public async Task<ActionResult> GetAll()
        {
            var model = await _context.Filmes.ToListAsync();
            return Ok(model);
        }

        [HttpPost]
        public async Task<ActionResult> Create(Filme model)
        {
            _context.Filmes.Add(model);
            await _context.SaveChangesAsync();
            return CreatedAtAction("GetById", new {id = model.Id}, model);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult> GetById(int id)
        {
            var model = await _context.Filmes
            .FirstOrDefaultAsync(c => c.Id == id);
            if (model == null) return NotFound();
            return Ok(model);

        }
        [HttpPut("{id}")]
        public async Task<ActionResult> Update(int id, Filme model)
        {
            if (id != model.Id) return BadRequest();

            var modeloDb = await _context.Filmes.AsNoTracking()
                .FirstOrDefaultAsync(c => c.Id == id);
            if (modeloDb == null) return NotFound();
            _context.Filmes.Update(model);
            await _context.SaveChangesAsync();
            return NoContent();

        }
        [HttpDelete("{id}")]
        public async Task<ActionResult> Delete(int id)
        {
            var model = await _context.Filmes.FindAsync(id);
            
            if (model == null) return NotFound();
            
            _context.Filmes.Remove(model);
            await _context.SaveChangesAsync();
            
            return NoContent();


        }
    }


}
