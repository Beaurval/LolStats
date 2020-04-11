using Newtonsoft.Json.Linq;
using PocketSummonner.Data;
using PocketSummonner.Models;
using PocketSummonner.Models.Profil;
using RestSharp;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace PocketSummonner.Controllers
{
    public class RequestState
    {
        // This class stores the state of the request.
        const int BUFFER_SIZE = 1024;
        public StringBuilder requestData;
        public byte[] bufferRead;
        public WebRequest request;
        public WebResponse response;
        public Stream responseStream;
        public RequestState()
        {
            bufferRead = new byte[BUFFER_SIZE];
            requestData = new StringBuilder("");
            request = null;
            responseStream = null;
        }
    }

    public class Invocateur_oldController : Controller
    {
        List<Task> allRequests = new List<Task>();
        private string api_key = Properties.Settings.Default.api_key;
        List<GameHistory> lastTenMatches = new List<GameHistory>();

        // GET: Invocateur
        public ActionResult Profil(string summonerId)
        {
            JObject jsonMatchs;

            //Requête
            WebRequest request = WebRequest.Create("https://euw1.api.riotgames.com/lol/match/v4/matchlists/by-account/" + summonerId + "?endIndex=10");
            request.Headers.Add("X-Riot-Token", api_key);

            //Réponse
            WebResponse response = request.GetResponse();
            using (Stream dataStream = response.GetResponseStream())
            {
                // Open the stream using a StreamReader for easy access.  
                StreamReader reader = new StreamReader(dataStream);
                // Read the content.  
                string responseFromServer = reader.ReadToEnd();

                jsonMatchs = JObject.Parse(responseFromServer);
            }

            foreach (JObject match in (JArray)jsonMatchs["matches"])
            {
                //Récupération d'info supplémentaire
                WebRequest moreInfoRequest = WebRequest.Create("https://euw1.api.riotgames.com/lol/match/v4/matches/" + match["gameId"].ToString());
                moreInfoRequest.Headers.Add("X-Riot-Token", api_key);

                RequestState rs = new RequestState();
                rs.request = moreInfoRequest;

                IAsyncResult r = (IAsyncResult)moreInfoRequest.BeginGetResponse(new AsyncCallback(GameResponseCallback), rs);

            }



            return View();
        }

        public ActionResult Recherche(string name)
        {
            ViewBag.name = name;
            return View();
        }



        private void GameResponseCallback(IAsyncResult asyncResult)
        {

            RequestState rs = (RequestState)asyncResult.AsyncState;
            WebRequest request = rs.request;

            rs.response = request.EndGetResponse(asyncResult);

            using (Stream dataStream = rs.response.GetResponseStream())
            {
                // Open the stream using a StreamReader for easy access.  
                StreamReader reader = new StreamReader(dataStream);
                // Read the content.  
                string responseFromServer = reader.ReadToEnd();
                JObject responseJson = JObject.Parse(responseFromServer);


                GameHistory game = new GameHistory();
                //game.Duration = Double.Parse(responseJson["gameDuration"].ToString()) / 60;

                lastTenMatches.Add(game);
                dataStream.Close();

            }

        }



    }
}