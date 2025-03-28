using static api_web_services_avaliacao_manager.Models.Filme;

namespace api_web_services_avaliacao_manager.Models
{
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;

    [Table("Usuarios")]
    public class Usuario : LinksHATEOS
    {
        public int Id { get; set; }
        [Required]
        public string Nome { get; set; }
        public string Email { get; set; } = string.Empty;
        public ICollection<Comentario> Comentarios { get; set; } = new List<Comentario>();
        public ICollection<Filme> Favoritos { get; set; } = new List<Filme>();
    }
}