using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
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
        public string Tier { get; set; }
        public string Rank { get; set; }
        public int LeaguePoints { get; set; }
        public int Win { get; set; }
        public int Losses { get; set; }
        public DateTime DateAjout { get; set; }


        //Relations
        public virtual List<Joueur> Joueurs { get; set; }
        public virtual List<Maitrise> Maitrises { get; set; }
        public virtual List<UserSawInvocateur> ConsulteParUser { get; set; }
    }
}