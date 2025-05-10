using api_web_services_avaliacao_manager.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using BCrypt.Net;
using System.Text.Json.Serialization;
using Microsoft.AspNetCore.Authorization;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace api_web_services_avaliacao_manager.Controllers
{
    [Authorize]
    [Route("api/Usuarios")]
    [ApiController]
    public class UsuariosController : ControllerBase
    {
        private readonly AppDbContext _context;
        public UsuariosController(AppDbContext context)
        {
            _context = context;
        }

        //GET: api/usuarios
        [HttpGet]
        public async Task<ActionResult> GetAll()
        {
            var Usuarios = await _context.Usuarios.Include(u => u.Favoritos).Include(u => u.Comentarios).ToListAsync();

            return Ok(Usuarios.Select(u => new UsuarioDTO
            {
                Id = u.Id,
                NomeCompleto = u.NomeCompleto,
                NomeDeUsuario = u.NomeDeUsuario,
                Email = u.Email,
                Comentarios = u.Comentarios,
                Favoritos = u.Favoritos
            }));
        }

        // GET: api/usuarios/{id}
        [HttpGet("{id}")]
        public async Task<ActionResult> GetById(int id)
        {
            var model = await _context.Usuarios.Include(u => u.Favoritos).Include(u => u.Comentarios)
               .AsNoTracking()
            .FirstOrDefaultAsync(c => c.Id == id);
            if (model == null) return NotFound();

            return Ok(new UsuarioDTO {
                Id = model.Id,
                NomeCompleto = model.NomeCompleto,
                NomeDeUsuario = model.NomeDeUsuario,
                Email = model.Email,
                Comentarios = model.Comentarios,
                Favoritos = model.Favoritos
            });

        }


        [AllowAnonymous]// POST: api/usuarios
        [HttpPost]
        public async Task<ActionResult> AddUsuario(Usuario usuario)
        {
            // Validação básica dos dados recebidos
            if (string.IsNullOrWhiteSpace(usuario.NomeCompleto) ||
                string.IsNullOrWhiteSpace(usuario.NomeDeUsuario) ||
                string.IsNullOrWhiteSpace(usuario.Email) ||
                string.IsNullOrWhiteSpace(usuario.Senha))
            {
                return BadRequest("Todos os campos são obrigatórios.");
            }

            // Verificar se o NomeDeUsuario ou Email já existem em uma única consulta
            var usuarioExistente = await _context.Usuarios
                .FirstOrDefaultAsync(u => u.NomeDeUsuario == usuario.NomeDeUsuario || u.Email == usuario.Email);

            if (usuarioExistente != null)
            {
                if (usuarioExistente.NomeDeUsuario == usuario.NomeDeUsuario)
                {
                    return BadRequest("Já existe um usuário com esse Nome de Usuário.");
                }

                if (usuarioExistente.Email == usuario.Email)
                {
                    return BadRequest("Já existe um usuário com esse E-mail.");
                }
            }

            // Criar o novo usuário com a senha criptografada
            var novoUsuario = new Usuario
            {
                NomeCompleto = usuario.NomeCompleto,
                NomeDeUsuario = usuario.NomeDeUsuario,
                Email = usuario.Email,
                Senha = BCrypt.Net.BCrypt.HashPassword(usuario.Senha)
            };

            // Adicionar o novo usuário ao contexto
            _context.Usuarios.Add(novoUsuario);
            await _context.SaveChangesAsync();

            // Retornar o resultado sem expor a senha
            return CreatedAtAction("GetById", new { id = novoUsuario.Id }, new
            {
                novoUsuario.Id,
                novoUsuario.NomeCompleto,
                novoUsuario.NomeDeUsuario,
                novoUsuario.Email
            });
        }

        // PUT: api/usuarios/{id}
        [HttpPut("{id}")]
        public async Task<ActionResult> Update(int id, Usuario model)
        {
            if (id != model.Id) return BadRequest();

            var modelDb = await _context.Usuarios.AsNoTracking()
                .FirstOrDefaultAsync(c => c.Id == id);
            if (modelDb == null) return NotFound();

            modelDb.NomeCompleto = model.NomeCompleto;
            modelDb.NomeDeUsuario = model.NomeDeUsuario;
            modelDb.Senha = BCrypt.Net.BCrypt.HashPassword(model.Senha);
            modelDb.Email = model.Email;

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

        [AllowAnonymous]
        [HttpPost("authenticate")]
       
        public async Task<ActionResult> Authenticate([FromBody] AuthenticateDto usuario)
        {
            // Verificar se o usuário existe no banco de dados
            var usuarioDb = await _context.Usuarios.FirstOrDefaultAsync(u => u.NomeDeUsuario == usuario.NomeDeUsuario);
            if (usuarioDb == null || !BCrypt.Net.BCrypt.Verify(usuario.Senha, usuarioDb.Senha))
            {
                return Unauthorized("Usuário ou senha incorretos.");
            }

            // Gerar o token JWT
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes("LoR$@NeA#*L2M0JsjVf4vSZ91Gcy2Qu#t"); // Use a mesma chave configurada no Program.cs
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new[]
                {
            new Claim(ClaimTypes.Name, usuarioDb.NomeDeUsuario),
            new Claim(ClaimTypes.NameIdentifier, usuarioDb.Id.ToString())
        }),
                Expires = DateTime.UtcNow.AddHours(2),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
            };
            var token = tokenHandler.CreateToken(tokenDescriptor);

            return Ok(new
            {
                Token = tokenHandler.WriteToken(token)

            });
        }


    }


}
