using System.ComponentModel.DataAnnotations;

namespace api_web_services_avaliacao_manager.Models
{
    public class AuthenticateDto
    {
        [Required]
        public string NomeDeUsuario { get; set; }

        [Required]
        public string Senha { get; set; }
    }
}
