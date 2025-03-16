namespace api_web_services_avaliacao_manager.Models
{
    using System.ComponentModel.DataAnnotations.Schema;
    [Table("Comentarios")]
    public class Comentario
    {
        public int Id { get; set; }
        public string? Texto { get; set; } = string.Empty; // (pode ser nulo se ainda nao houver comentarios)
        public int UsuarioId { get; set; }
        public required Usuario Usuario { get; set; }
        public int FilmeId { get; set; }
        public required Filme Filme { get; set; }
    }
}
