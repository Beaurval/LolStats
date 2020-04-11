using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace PocketSummonner.Models
{
    public class InvocateurModel
    {
        //Summoner infos
        public string Name { get; set; }
        public string Puuid { get; set; }
        public string AccountID { get; set; }
        public string Id { get; set; }
        public int Niveau { get; set; }
        public int ProfilIconId { get; set; }
        public string ProfilIconUrl { get; set; }

        //Ranked infos
        public string QueueType { get; set; }
        public bool HotStreak { get; set; }
        public JObject MiniSeries { get; set; }
        public int Wins { get; set; }
        public bool Veteran { get; set; }
        public int Losses { get; set; }
        public string Rank { get; set; }
        public string Tier { get; set; }
        public bool Inactive { get; set; }
        public bool FreshBlood { get; set; }
        public string LeagueId { get; set; }
        public int LeaguePoints { get; set; }
    }
}