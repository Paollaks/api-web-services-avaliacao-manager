using Microsoft.EntityFrameworkCore;

namespace api_web_services_avaliacao_manager.Models
{
    public class AppDbContext : DbContext 
    {
        public AppDbContext(DbContextOptions options) : base(options)
        {         
        }

        public DbSet<Filme> Filmes { get; set; }
        public DbSet<Comentario> Comentarios { get; set; }
        public DbSet<Usuario> Usuarios { get; set; }
    }
}
