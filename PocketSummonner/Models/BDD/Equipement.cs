using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace PocketSummonner.Models.BDD
{
    public class Equipement
    {
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int Id { get; set; }
        public string Nom { get; set; }
        public string Image { get; set; }
        public string Description { get; set; 
        }
        public List<Joueur> Joueurs { get; set; }
    }
}