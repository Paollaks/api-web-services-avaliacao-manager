namespace api_web_services_avaliacao_manager.Models
{
    using System.ComponentModel.DataAnnotations.Schema;
    using System.ComponentModel.DataAnnotations;
    using Microsoft.EntityFrameworkCore;
    using api_web_services_avaliacao_manager.Models;

    [Table("Comentarios")]
    public class Comentario : LinksHATEOS
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public string Texto { get; set; }

        public int IdUsuario { get; set; }

        // Adicionando a referência ao Id do TMDBFilme
        public int TMDBFilmeId { get; set; }
    }
}
