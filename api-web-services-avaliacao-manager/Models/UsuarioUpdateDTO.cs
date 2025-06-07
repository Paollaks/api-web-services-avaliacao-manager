using System.ComponentModel.DataAnnotations;

namespace api_web_services_avaliacao_manager.Models
{
    public class UsuarioUpdateDTO
    {
        [Required]
        public int Id { get; set; }

        [Required]
        [MaxLength(30)]
        public string NomeCompleto { get; set; }

        [Required]
        [EmailAddress]
        [MaxLength(50)]
        public string Email { get; set; }

    }
}
