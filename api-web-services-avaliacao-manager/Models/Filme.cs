namespace api_web_services_avaliacao_manager.Models
{
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;

    
    public class Filme : LinksHATEOS
    {
        public int Id { get; set; }  

        [Required]
        public string Titulo { get; set; }  

        public int AnoLancamento { get; set; }  

        public string? Genero { get; set; }  

        public string? Sinopse { get; set; }
        public int IdFilme { get; internal set; }
    }
}
