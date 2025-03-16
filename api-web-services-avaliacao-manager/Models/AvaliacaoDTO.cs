namespace api_web_services_avaliacao_manager.Models
{
    public class AvaliacaoDTO
    {
        public int FilmeId { get; set; }
        public int UsuarioId { get; set; }
        public int Nota { get; set; }
        public string Comentario { get; set; } = string.Empty;
    }

}