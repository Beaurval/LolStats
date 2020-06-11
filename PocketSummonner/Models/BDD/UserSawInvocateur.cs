using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace PocketSummonner.Models.BDD
{
    public class UserSawInvocateur
    {
        public int UserId { get; set; }
        public string InvocateurId { get; set; }

        public DateTime DateConsultation { get; set; }

        public virtual User User { get; set; }
        public virtual Invocateur Invocateur { get; set; }
    }
}