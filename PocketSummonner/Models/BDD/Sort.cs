using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace PocketSummonner.Models.BDD
{
    public class Sort
    {
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int Id { get; set; }
        public string UrlImage { get; set; }
        //Relations
        public virtual List<Joueur> Joueurs { get; set; }
    }
}