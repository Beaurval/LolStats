using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace PocketSummonner.Models.BDD
{
    public class Partie
    {
        public long Id { get; set; }
        public bool Victoire { get; set; }
        public int Duree { get; set; }
        public string TypePartie { get; set; }
        public DateTime DatePartie { get; set; }


        //Relations
        public virtual Joueur Joueur { get; set; }
        public virtual List<Joueur> AutreJoueurs { get; set; }
    }
}