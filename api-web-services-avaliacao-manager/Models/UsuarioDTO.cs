using static api_web_services_avaliacao_manager.Models.Filme;

namespace api_web_services_avaliacao_manager.Models
{
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using BCrypt.Net;
    using System.Text.Json.Serialization;

    public class UsuarioDTO : LinksHATEOS
    {
        public int Id { get; set; }
        public string NomeCompleto { get; set; }
        public string NomeDeUsuario { get; set; }
        public string Email { get; set; }
        public ICollection<Comentario> Comentarios { get; set; } = new List<Comentario>();
        public ICollection<Favorito> Favoritos { get; set; } = new List<Favorito>();
    }
}