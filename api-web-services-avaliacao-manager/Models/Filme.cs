﻿namespace api_web_services_avaliacao_manager.Models
{
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System;

    [Table("Filmes")]
    public class Filme : LinksHATEOS
    {
        public int Id { get; set; }
        [Required]
        public string Titulo { get; set; }
        public int AnoLancamento { get; set; }
        public string? Genero { get; set; }
        public string? Sinopse { get; set; }
        public double? NotaMedia { get; set; } // 0 a 5 estrelas (pode ser nulo se ainda nao houver avaliaçoes)
        public int? NumeroAvaliacoes { get; set; } // (pode ser nulo se ainda nao houver avaliaçoes)
        public ICollection<Comentario> Comentarios { get; set; } = new List<Comentario>();
        public ICollection<Usuario> UsuariosFavoritaram { get; set; } = new List<Usuario>();
    }

       

}