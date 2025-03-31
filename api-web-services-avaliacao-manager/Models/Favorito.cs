using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore;
using api_web_services_avaliacao_manager.Models;

namespace api_web_services_avaliacao_manager.Models
{
    [Table("Favoritos")]

    [PrimaryKey(nameof(IdUsuario), nameof(IdFilme))]
    public class Favorito
    {
        public int IdUsuario { get; set; }
        public int IdFilme { get; set; }

        // Relacionamentos com Usuario e Filme
        [ForeignKey("IdUsuario")]
        public Usuario Usuario { get; set; }

        [ForeignKey("IdFilme")]
        public Filme Filme { get; set; }
    }
}