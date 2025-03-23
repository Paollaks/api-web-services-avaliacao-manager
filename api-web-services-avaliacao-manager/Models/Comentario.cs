namespace api_web_services_avaliacao_manager.Models
{
    using System.ComponentModel.DataAnnotations.Schema;
    [Table("Comentarios")]
    public class Comentario : LinksHATEOS
    {
        public int Id { get; set; }
        public string? Texto { get; set; } = string.Empty; // (pode ser nulo se ainda nao houver comentarios)
        public int UsuarioId { get; set; }
        public Usuario Usuario { get; set; }
        public int FilmeId { get; set; }
       
    }
}
