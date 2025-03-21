namespace api_web_services_avaliacao_manager.Models
{
    public class LinkDTO
    {
        public int Id { get; set; }
        public string Href { get; set; }
        public string Rel { get; set; }
        public string Metodo { get; set; }
        public LinkDTO(int id, string href, string rel, string metodo)
        {
            Id = id;
            Href = href;
            Rel = rel;
            Metodo = metodo;
        }

    }

    public class LinksHATEOS
    {
        public List<LinkDTO> Links { get; set; } = new List<LinkDTO>();
    }
}
