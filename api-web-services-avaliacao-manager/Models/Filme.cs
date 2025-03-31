namespace api_web_services_avaliacao_manager.Models
{
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;

    
    public class Filme : LinksHATEOS
    {

        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.None)] // Permite definir o ID manualmente
        public int Id { get; set; }  

        [Required]
        public string Titulo { get; set; }  

        public int AnoLancamento { get; set; }  

        public string Genero { get; set; }  

        public string Sinopse { get; set; }

        // Relacionamento N:N com Usuario (tabela de junção Favorito)
        public ICollection<Favorito> UsuariosFavoritaram { get; set; } = new List<Favorito>();

        // Relacionamento N:N com Usuario (tabela de junção Comentario)
        public ICollection<Comentario> Comentarios { get; set; } = new List<Comentario>();
    }
}
