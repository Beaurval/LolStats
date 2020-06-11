using Microsoft.Ajax.Utilities;
using Newtonsoft.Json.Linq;
using PocketSummonner.Models.BDD;
using PocketSummonner.Models.Helpers;
using PocketSummonner.Models.Profil;
using RestSharp;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Runtime.InteropServices.ComTypes;
using System.Threading.Tasks;
using System.Web;

namespace PocketSummonner.Helpers
{
    public  class Fonctions
    {
        private HttpRequestBase request;
        public Fonctions(HttpRequestBase rqst)
        {
            request = rqst;
        }
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

        public User CurrentUser()
        {
                DataContext db = new DataContext();
                User user = db.Users.Find(Int32.Parse(request.Cookies.Get("UserId").Value));
                return user;
        }
    }
}