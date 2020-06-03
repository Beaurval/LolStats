using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace PocketSummonner.Models.BDD
{
    public class Invocateur
    {
        public string Id { get; set; }
        public string AccountId { get; set; }
        public string Name { get; set; }
        public int ImageProfil { get; set; }
        public int Niveau { get; set; }
        public string Region { get; set; }

        //Relations
        public virtual List<Joueur> Joueurs { get; set; }
    }
}