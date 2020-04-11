using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace PocketSummonner.Models.Profil
{
    public class Player
    {
        public int idParticipant { get; set; }
        public int idTeam { get; set; }
        public string idSummoner { get; set; }
        public string ImageChampion{ get; set; }
        public string Name { get; set; }
    }
}