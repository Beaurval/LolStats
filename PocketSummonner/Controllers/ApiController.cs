using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Web;
using System.Web.Mvc;

namespace PocketSummonner.Controllers
{
    public class ApiController : Controller
    {
        String[] servers;
        string api_key = Properties.Settings.Default.api_key.ToString();

        public ApiController()
        {
            servers = new string[11]{
                "br1",
                "eun1",
                "euw1",
                "jp1",
                "kr",
                "la1",
                "la2",
                "na1",
                "oc1",
                "tr1",
                "ru"
            };
        }

        public WebRequest GetRequest(string server, string route)
        {
            WebRequest request = WebRequest.Create("https://" + server + ".api.riotgames.com" + route);
            request.Credentials = CredentialCache.DefaultCredentials;
            request.Headers.Add("X-Riot-Token", api_key);
            request.ContentType = "application/json;charset=utf-8";

            return request;
        }




        public JArray SumByName(string name)
        {
            JArray listPlayers = new JArray();

            foreach (string server in servers)
            {
                try
                {
                    WebRequest request = GetRequest(server, "/lol/summoner/v4/summoners/by-name/" + name);
                    request.Method = "GET";

                    WebResponse response = request.GetResponse();
                    var responseString = new StreamReader(response.GetResponseStream()).ReadToEnd();

                    JObject player = JObject.Parse(responseString);

                    player["region"] = server;
                    listPlayers.Add(player);
                }
                catch
                {

                }
            }



            return listPlayers;
        }

    }
}