using static api_web_services_avaliacao_manager.Models.Filme;

namespace api_web_services_avaliacao_manager.Models
{
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;

    [Table("Usuarios")]
    public class Usuario : LinksHATEOS
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [MaxLength(50)]
        public string Nome { get; set; }

        [Required]
        [EmailAddress]
        [MaxLength(50)]
        public string Email { get; set; } = string.Empty;

        [Required]
        [MaxLength(20)]
        public string Senha { get; set; }

        // Relacionamento N:N com Filme (tabela de junção Favorito)
        public ICollection<Favorito> Favoritos { get; set; } = new List<Favorito>();

        // Relacionamento N:N com Filme (tabela de junção Comentario)
        public ICollection<Comentario> Comentarios { get; set; } = new List<Comentario>();
    }
}