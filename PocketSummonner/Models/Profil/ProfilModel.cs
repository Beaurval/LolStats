using PocketSummonner.Models.BDD;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace PocketSummonner.Models.Profil
{
    public class ProfilModel
    {
        public List<Joueur> DernieresParties { get; set; }
        public List<Maitrise> Maitrises { get; set; }
        public List<UserSawInvocateur> History { get; set; }
    }
}