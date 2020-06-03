using Newtonsoft.Json.Linq;
using PocketSummonner.Models.BDD;
using RestSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web;

namespace PocketSummonner.Helpers
{
    public static class ApiCall
    {
        private static string api_key = Properties.Settings.Default.api_key;

        public async static Task<Invocateur> GetSummoner(string summonerId)
        {
            HttpClient client = new HttpClient();
            client.DefaultRequestHeaders.Add("X-Riot-Token", api_key);

            var response = await client.GetAsync("https://euw1.api.riotgames.com/lol/summoner/v4/summoners/" + summonerId);
            Invocateur invocateur = new Invocateur();
            do
            {
                string content = await response.Content.ReadAsStringAsync();
                JObject jsonInvocateur = JObject.Parse(content);
                if (response.IsSuccessStatusCode)
                {
                    invocateur = new Invocateur
                    {
                        Id = jsonInvocateur["id"].ToString(),
                        AccountId = jsonInvocateur["accountId"].ToString(),
                        Name = jsonInvocateur["name"].ToString(),
                        ImageProfil = Int32.Parse(jsonInvocateur["profileIconId"].ToString()),
                        Niveau = Int32.Parse(jsonInvocateur["summonerLevel"].ToString()),
                        Region = "EUW1",
                        Joueurs = new List<Joueur>()
                    };
                }
                else
                    System.Threading.Thread.Sleep(1000 * 60 * 2);
            } while ((int)response.StatusCode == 429);

            return invocateur;
        }


        public async static Task<List<Joueur>> GetGameHistory(string accountId, DataContext db)
        {
            List<Joueur> history = new List<Joueur>();

            HttpClient client = new HttpClient();
            client.DefaultRequestHeaders.Add("X-Riot-Token", api_key);

            var response = await client.GetAsync("https://euw1.api.riotgames.com/lol/match/v4/matchlists/by-account/" + accountId + "?endIndex=5");

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

                        Partie partie = new Partie();

                        //Liste des joueurs
                        partie.Joueurs = new List<Joueur>();


                        partie.DatePartie = UnixTimeStampToDateTime(Double.Parse(jsonMatch["gameCreation"].ToString()) / 1000);
                        partie.Duree = Int32.Parse(jsonMatch["gameDuration"].ToString());
                        partie.TypePartie = jsonMatch["gameMode"].ToString();

                        //Récupération informations des joueurs de la game
                        foreach (JObject playersIdentities in (JArray)jsonMatch["participantIdentities"])
                        {
                            Joueur j = new Joueur();
                            j.Equipements = new List<Equipement>();
                            string invocateurId = playersIdentities["player"]["summonerId"].ToString();
                            Invocateur invocateurJoueur = SaveToDb.SaveInvocateur(
                                await GetSummoner(invocateurId), db);
                            j.IdParticipant = Int32.Parse(playersIdentities["participantId"].ToString());
                            j.Invocateur = invocateurJoueur;


                            //Récupération des stats
                            foreach (JObject playersStats in (JArray)jsonMatch["participants"])
                            {
                                int idParticipant = Int32.Parse(playersStats["participantId"].ToString());
                                if (idParticipant == j.IdParticipant)
                                {
                                    int idChamp = Int32.Parse(playersStats["championId"].ToString());
                                    j.Champion = db.Champions.Find(idChamp);


                                    j.EquipeId = Int32.Parse(playersStats["teamId"].ToString());
                                    j.Sort1 = db.Sorts.Find(Int32.Parse(playersStats["spell1Id"].ToString()));
                                    j.Sort2 = db.Sorts.Find(Int32.Parse(playersStats["spell2Id"].ToString()));
                                    j.Victoire = Boolean.Parse(playersStats["stats"]["win"].ToString());
                                    j.NbTue = Int32.Parse(playersStats["stats"]["kills"].ToString());
                                    j.NbMort = Int32.Parse(playersStats["stats"]["deaths"].ToString());
                                    j.NbAssist = Int32.Parse(playersStats["stats"]["assists"].ToString());
                                    j.NbSbire = Int32.Parse(playersStats["stats"]["totalMinionsKilled"].ToString());
                                    j.Level = Int32.Parse(playersStats["stats"]["champLevel"].ToString());
                                    j.KDA = j.NbMort != 0 ? (j.NbTue + j.NbAssist) / j.NbMort : 0;
                                    j.Poste = playersStats["timeline"]["lane"].ToString();

                                    for (int i = 0; i < 6; i++)
                                    {
                                        int itemId;
                                        try
                                        {
                                            itemId = Int32.Parse(playersStats["stats"]["item" + i].ToString());
                                        }
                                        catch (Exception)
                                        {
                                            itemId = 0;
                                        } 


                                        if (itemId != 0)
                                            j.Equipements.Add(db.Equipements.Find(itemId));
                                        else
                                        {
                                            j.Equipements.Add(db.Equipements.Find(1));
                                        }
                                    }
                                }
                            }

                            j.Partie = partie;


                            if (j.Invocateur.AccountId == accountId)
                                history.Add(j);
                            partie.Joueurs.Add(j);
                        }
                    }
                }
            }
            return history;
        }


        public async static Task<List<Sort>> GetSpells()
        {
            List<Sort> sorts = new List<Sort>();
            HttpClient client = new HttpClient();
            client.DefaultRequestHeaders.Add("X-Riot-Token", api_key);

            var response = await client.GetAsync("http://ddragon.leagueoflegends.com/cdn/10.11.1/data/en_US/summoner.json");

            if (response.IsSuccessStatusCode)
            {
                string content = await response.Content.ReadAsStringAsync();
                JObject jsonSpells = JObject.Parse(content);

                foreach (JProperty x in (JToken)jsonSpells["data"])
                { // if 'obj' is a JObject


                    sorts.Add(new Sort
                    {
                        Id = Int32.Parse(x.ElementAt(0)["key"].ToString()),
                        UrlImage = "http://ddragon.leagueoflegends.com/cdn/10.11.1/img/spell/" + x.ElementAt(0)["image"]["full"].ToString()
                    });
                }
            }
            return sorts;
        }


        public async static Task<List<Champion>> GetChampions()
        {
            List<Champion> champions = new List<Champion>();
            HttpClient client = new HttpClient();
            client.DefaultRequestHeaders.Add("X-Riot-Token", api_key);

            var response = await client.GetAsync("http://ddragon.leagueoflegends.com/cdn/10.11.1/data/en_US/champion.json");

            if (response.IsSuccessStatusCode)
            {
                string content = await response.Content.ReadAsStringAsync();
                JObject jsonChamp = JObject.Parse(content);

                foreach (JProperty x in (JToken)jsonChamp["data"])
                { // if 'obj' is a JObject


                    champions.Add(new Champion
                    {
                        Id = Int32.Parse(x.ElementAt(0)["key"].ToString()),
                        Image = "http://ddragon.leagueoflegends.com/cdn/10.11.1/img/champion/" + x.ElementAt(0)["image"]["full"].ToString(),
                        Nom = x.ElementAt(0)["name"].ToString()
                    });
                }
            }
            return champions;
        }

        public async static Task<List<Equipement>> GetEquipements()
        {
            List<Equipement> equipements = new List<Equipement>();
            HttpClient client = new HttpClient();
            client.DefaultRequestHeaders.Add("X-Riot-Token", api_key);

            var response = await client.GetAsync("http://ddragon.leagueoflegends.com/cdn/10.11.1/data/en_US/item.json");

            if (response.IsSuccessStatusCode)
            {
                string content = await response.Content.ReadAsStringAsync();
                JObject jsonChamp = JObject.Parse(content);

                foreach (JProperty x in (JToken)jsonChamp["data"])
                { // if 'obj' is a JObject
                    equipements.Add(new Equipement
                    {
                        Id = Int32.Parse(x.Name),
                        Image = "http://ddragon.leagueoflegends.com/cdn/10.11.1/img/item/" + x.ElementAt(0)["image"]["full"].ToString(),
                        Nom = x.ElementAt(0)["name"].ToString(),
                        Description = x.ElementAt(0)["description"].ToString()
                    });
                }
            }
            return equipements;
        }



        public static DateTime UnixTimeStampToDateTime(double unixTimeStamp)
        {
            // Unix timestamp is seconds past epoch
            System.DateTime dtDateTime = new DateTime(1970, 1, 1, 0, 0, 0, 0, System.DateTimeKind.Utc);
            dtDateTime = dtDateTime.AddSeconds(unixTimeStamp).ToLocalTime();
            return dtDateTime;
        }
    }
}