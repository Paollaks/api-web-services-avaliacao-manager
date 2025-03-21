namespace api_web_services_avaliacao_manager.Models
{
    public class AvaliacaoDTO
    {
        public int FilmeId { get; set; }  // ID do filme
        public string TituloFilme { get; set; } // 🔹 Título do filme
        public string Usuario { get; set; } // Nome de usuário do TMDB
        public double? Nota { get; set; } // Nota do usuário (pode ser nulo)
        public string Comentario { get; set; } // Conteúdo da avaliação
    }
}