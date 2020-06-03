using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace PocketSummonner.Models.BDD
{
    public class Equipe
    {
        public int Id { get; set; }
        public string Color { get; set; }

        //Relations
        public virtual List<Joueur> Joueurs { get; set; }
    }
}