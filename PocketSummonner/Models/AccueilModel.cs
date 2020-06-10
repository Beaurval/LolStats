using PocketSummonner.Models.BDD;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace PocketSummonner.Models
{
    public class AccueilModel
    {
        public List<Invocateur> lastInvocateeur { get; set; }
        public List<Invocateur> topMasteries { get; set; }
    }
}