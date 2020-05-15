using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace PocketSummonner.Models.BDD
{
    public class Joueur
    {
        public int Id { get; set; }   
        public int IdParticipant { get; set; }
        public string Sort1 { get; set; }
        public string Sort2 { get; set; }
        public int NbTue { get; set; }
        public int NbMort { get; set; }
        public int NbAssist { get; set; }
        public int NbSbire { get; set; }
        public int Level { get; set; }
        public string Poste { get; set; }
        public double KDA { get; set; }
        //Relations
        public virtual Invocateur Invocateur { get; set; }
        public virtual List<Equipement> Equipements { get; set; }
    }
}