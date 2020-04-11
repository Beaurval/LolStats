using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace PocketSummonner.Models.Profil
{
    public class GameHistory
    {
        public bool Victory { get; set; }
        public int idParticipantPrincial { get; set; }
        public DateTime HistoryDate { get; set; }
        public string TypeGame { get; set; }
        public int Duration { get; set; }
        public string ImgChampion { get; set; }
        public string ImgSpell1 { get; set; }
        public string ImgSpell2 { get; set; }
        public List<Item> Items { get; set; }
        public List<Player> Players { get; set; }
        public int Kills { get; set; }
        public int Deaths { get; set; }
        public int Assists { get; set; }
        public int Level { get; set; }
        public int Creeps { get; set; }
        public double KDA { get; set; }
        public string Lane { get; set; }
    }
}