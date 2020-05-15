using Newtonsoft.Json.Linq;
using PocketSummonner.Models.BDD;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace PocketSummonner.Helpers
{
    public static class ConvertJson
    {
        public static Invocateur ConvertJsonSummoner(JObject jsonSummonner)
        {
            Invocateur invocateur = new Invocateur
            {
                Id = jsonSummonner["id"].ToString(),
                AccountId = jsonSummonner["accountId"].ToString(),
                Name = jsonSummonner["name"].ToString(),
                ImageProfil = Int32.Parse(jsonSummonner["profileIconId"].ToString()),
                DernieresParties = new List<Partie>()
            };

            return invocateur;
        }

    }
}