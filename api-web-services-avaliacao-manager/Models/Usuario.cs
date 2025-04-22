using static api_web_services_avaliacao_manager.Models.Filme;

namespace api_web_services_avaliacao_manager.Models
{
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using BCrypt.Net;

    [Table("Usuarios")]
    public class Usuario : LinksHATEOS
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [MaxLength(50)]
        public string NomeCompleto { get; set; }

        [Required]
        [MaxLength(30)]
        public string NomeDeUsuario { get; set; }

        [Required]
        [EmailAddress]
        [MaxLength(50)]
        public string Email { get; set; } 

        [Required]
        public string Senha { get; set; }

        public ICollection<Favorito> Favoritos { get; set; } = new List<Favorito>();
        public ICollection<Comentario> Comentarios { get; set; } = new List<Comentario>();


        public void SetSenha(string senha)
        {
            Senha = BCrypt.HashPassword(senha);
        }

        public bool VerificarSenha(string senha)
        {
            return BCrypt.Verify(senha, Senha);
        }
    }
}