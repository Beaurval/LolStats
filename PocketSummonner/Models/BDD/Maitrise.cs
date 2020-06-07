using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace PocketSummonner.Models.BDD
{
    public class Maitrise
    {
        public int Id { get; set; }
        public int Niveau { get; set; }
        public long ChampionXp { get; set; }
        public int XpRestanteProchainNiveau { get; set; }


        //Relations
        public virtual Champion Champion { get; set; }
        public virtual Invocateur Invocateur { get; set; }
    }
}