using api_web_services_avaliacao_manager.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace api_web_services_avaliacao_manager.Controllers
{
    [Route("api/usuarios")]
    [ApiController]
    public class UsuariosController : ControllerBase
    {
        private readonly AppDbContext _context;
        public UsuariosController (AppDbContext context)
        {
            _context = context;
        }

        //GET: api/usuarios
        [HttpGet]
        public async Task<ActionResult> GetAll()
        {
            var usuarios = await _context.Usuarios.Include(u => u.Favoritos).Include(u => u.Comentarios).ToListAsync();
            return Ok(usuarios);
        }

        // GET: api/usuarios/{id}
        [HttpGet("{id}")]
        public async Task<ActionResult> GetById(int id)
        {
            var model = await _context.Usuarios
            .FirstOrDefaultAsync(c => c.Id == id);
            if (model == null) return NotFound();

            return Ok(model);

        }


        // POST: api/usuarios
        [HttpPost]
        public async Task<ActionResult> AddUsuario(Usuario usuario)
        {
            // Verifica se já existe um usuário com o mesmo e-mail e o mesmo idNome
            if (_context.Usuarios.Any(u => u.Email == usuario.Email))
            {
                return BadRequest("Já existe um usuário com esse e-mail.");
            }

            try
            {
                _context.Usuarios.Add(usuario);
                await _context.SaveChangesAsync();

                return CreatedAtAction(nameof(AddUsuario), new { id = usuario.Id }, usuario);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        // PUT: api/usuarios/{id}
        [HttpPut("{id}")]
        public async Task<ActionResult> Update(int id, Usuario model)
        {
            if (id != model.Id) return BadRequest();

            var modeloDb = await _context.Usuarios.AsNoTracking()
                .FirstOrDefaultAsync(c => c.Id == id);
            if (modeloDb == null) return NotFound();
            _context.Usuarios.Update(model);
            await _context.SaveChangesAsync();
            return NoContent();

        }

        // DELETE: api/usuarios/{id}
        [HttpDelete("{id}")]
        public async Task<ActionResult> Delete(int id)
        {
            var model = await _context.Usuarios.FindAsync(id);

            if (model == null) return NotFound();

            _context.Usuarios.Remove(model);
            await _context.SaveChangesAsync();

            return NoContent();


        }
    
    }


}
