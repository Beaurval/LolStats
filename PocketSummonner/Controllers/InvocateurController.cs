﻿using Newtonsoft.Json.Linq;
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
using System.Web.Hosting;
using System.Web.Mvc;

namespace PocketSummonner.Controllers
{
    public class InvocateurController : Controller
    {
        private string api_key = Properties.Settings.Default.api_key;

        public static DateTime UnixTimeStampToDateTime(double unixTimeStamp)
        {
            // Unix timestamp is seconds past epoch
            System.DateTime dtDateTime = new DateTime(1970, 1, 1, 0, 0, 0, 0, System.DateTimeKind.Utc);
            dtDateTime = dtDateTime.AddSeconds(unixTimeStamp).ToLocalTime();
            return dtDateTime;
        }

        //// GET: Invocateur
        public async Task<ActionResult> Profil(string summonerId)
        {
            List<GameHistory> model = new List<GameHistory>();
            HttpClient client = new HttpClient();
            client.DefaultRequestHeaders.Add("X-Riot-Token", api_key);

            var response = await client.GetAsync("https://euw1.api.riotgames.com/lol/match/v4/matchlists/by-account/" + summonerId + "?endIndex=10");

            string content = await response.Content.ReadAsStringAsync();
            if (response.IsSuccessStatusCode)
            {
                JObject jsonMatchList = JObject.Parse(content);

                foreach (JObject match in (JArray)jsonMatchList["matches"])
                {
                    var responseMatch = await client.GetAsync("https://euw1.api.riotgames.com/lol/match/v4/matches/"
                        + match["gameId"].ToString());

                    string contentMatch = await responseMatch.Content.ReadAsStringAsync();
                    if (responseMatch.IsSuccessStatusCode)
                    {
                        JObject jsonMatch = JObject.Parse(contentMatch);
                        GameHistory game = new GameHistory();
                        game.Players = new List<Player>();
                        game.Items = new List<Item>();

                        game.HistoryDate = UnixTimeStampToDateTime(Double.Parse(jsonMatch["gameCreation"].ToString())/1000);
                        game.Duration = Int32.Parse(jsonMatch["gameDuration"].ToString());
                        game.TypeGame = jsonMatch["gameMode"].ToString();

                        //Récupération des identités des joueurs de la game
                        foreach (JObject playersIdentities in (JArray)jsonMatch["participantIdentities"])
                        {
                            Player pl = new Player();

                            pl.Name = playersIdentities["player"]["summonerName"].ToString();
                            pl.idSummoner = playersIdentities["player"]["accountId"].ToString();
                            pl.idParticipant = Int32.Parse(playersIdentities["participantId"].ToString());
                            if (pl.idSummoner == summonerId)
                                game.idParticipantPrincial = pl.idParticipant;
                            game.Players.Add(pl);
                        }

                        //Récupération des stats invocateur principal et teams
                        foreach (JObject playersStats in (JArray)jsonMatch["participants"])
                        {
                            int idParticipant = Int32.Parse(playersStats["participantId"].ToString());
                            string nameChampion = await Helpers.IdToName.GetChampionName(Int32.Parse(playersStats["championId"].ToString()));
                            //Récup des infos du participant
                            Player pl = game.Players.Where(x => x.idParticipant == idParticipant).First();
                            pl.idTeam = Int32.Parse(playersStats["teamId"].ToString());
                            pl.ImageChampion = "http://ddragon.leagueoflegends.com/cdn/10.6.1/img/champion/"
                                + nameChampion + ".png";
                            //Récup info invocateur pricipal
                            if (idParticipant == game.idParticipantPrincial)
                            {
                                //Victoire ?
                                game.Victory = bool.Parse(playersStats["stats"]["win"].ToString());
                                //Spells invocateur
                                game.ImgSpell1 = "http://ddragon.leagueoflegends.com/cdn/10.6.1/img/spell/" +
                                    await Helpers.IdToName.GetSpellName(Int32.Parse(playersStats["spell1Id"].ToString()))
                                    + ".png";
                                game.ImgSpell2 = "http://ddragon.leagueoflegends.com/cdn/10.6.1/img/spell/" +
                                    await Helpers.IdToName.GetSpellName(Int32.Parse(playersStats["spell2Id"].ToString()))
                                    + ".png";

                                //Img du champion
                                game.ImgChampion = pl.ImageChampion;
                                game.Kills = Int32.Parse(playersStats["stats"]["kills"].ToString());
                                game.Deaths = Int32.Parse(playersStats["stats"]["deaths"].ToString());
                                game.Assists = Int32.Parse(playersStats["stats"]["assists"].ToString());
                                game.Creeps = Int32.Parse(playersStats["stats"]["totalMinionsKilled"].ToString());
                                game.Level = Int32.Parse(playersStats["stats"]["champLevel"].ToString());
                                game.KDA = (game.Kills + game.Assists) / game.Deaths;
                                game.Lane = playersStats["timeline"]["lane"].ToString();

                                for (int i = 0; i < 6; i++)
                                {
                                    string path = VirtualPathUtility.ToAbsolute("~/Content/img/assets/empty.png");

                                    Item item = await Helpers.IdToName.GetItem(Int32.Parse(playersStats["stats"]["item"+i].ToString()));
                                    game.Items.Add(item);
                                }
                            }

                        }
                        model.Add(game);
                    }

                }
            }

            return View(model);
        }

        public ActionResult Recherche(string name)
        {
            ViewBag.name = name;
            return View();
        }

    }
}