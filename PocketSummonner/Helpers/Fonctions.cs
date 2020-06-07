using Newtonsoft.Json.Linq;
using PocketSummonner.Models.BDD;
using PocketSummonner.Models.Profil;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web;

namespace PocketSummonner.Helpers
{
    public static class Fonctions
    {
        public static string TierColor(string tier)
        {
            string colorCode = "#ffffff";

            switch (tier)
            {
                case "IRON":
                    {
                        colorCode = "#A19D94";
                        break;
                    }
                case "BRONZE":
                    {
                        colorCode = "#B87333";
                        break;
                    }
                case "SILVER":
                    {
                        colorCode = "silver";
                        break;
                    }
                case "GOLD":
                    {
                        colorCode = "gold";
                        break;
                    }
                case "PLATINIUM":
                    {
                        colorCode = "#E5E4E2";
                        break;
                    }
                case "DIAMOND":
                    {
                        colorCode = "#B9F2FF";
                        break;
                    }
            }
            return colorCode;
        }
    }
}