using Newtonsoft.Json.Linq;
using PocketSummonner.Models.BDD;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Net.Http;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace PocketSummonner.Helpers
{
    public class ApiCall
    {
        private static string api_key = Properties.Settings.Default.api_key;
        private DataContext db;
        private HttpCall call;

        public ApiCall(DataContext context,HttpCall caller)
        {
            this.db = context;
            call = caller; 
        }

        public static async Task<Invocateur> GetSummoner(string summonerId,string serveur, HttpCall call, DataContext db)
        {
            Invocateur invocateur = new Invocateur();

            JArray rankedInfo = await call.HttpGetJArray("https://" + serveur  + ".api.riotgames.com/lol/league/v4/entries/by-summoner/" + summonerId);
            JObject jsonInvocateur = await call.HttpGetJObject("https://" + serveur + ".api.riotgames.com/lol/summoner/v4/summoners/" + summonerId);

            invocateur = new Invocateur
            {
                Id = jsonInvocateur["id"].ToString(),
                AccountId = jsonInvocateur["accountId"].ToString(),
                Name = jsonInvocateur["name"].ToString(),
                ImageProfil = Int32.Parse(jsonInvocateur["profileIconId"].ToString()),
                Niveau = Int32.Parse(jsonInvocateur["summonerLevel"].ToString()),
                Region = serveur,
                DateAjout = DateTime.Now,
                Joueurs = new List<Joueur>()
            };

            if (rankedInfo.Count > 0)
            {
                foreach (JObject r in rankedInfo)
                {
                    if (r["queueType"].ToString() == "RANKED_SOLO_5x5")
                    {
                        invocateur.LeaguePoints = Int32.Parse(r["leaguePoints"].ToString());
                        invocateur.Win = Int32.Parse(r["wins"].ToString());
                        invocateur.Losses = Int32.Parse(r["losses"].ToString());
                        invocateur.Rank = r["rank"].ToString();
                        invocateur.Tier = r["tier"].ToString();
                    }
                    else
                    {
                        invocateur.LeaguePoints = 0;
                        invocateur.Win = 0;
                        invocateur.Losses = 0;
                        invocateur.Rank = "";
                        invocateur.Tier = "UNRANKED";
                    }
                }
            }

            return invocateur;
        }
        public static async Task MajMaitrises(string summId,DataContext db)
        {
            Invocateur i = await db.Invocateurs.FindAsync(summId);

            i.Maitrises = await GetMaitrise(summId, db);
            db.Maitrises.AddRange(i.Maitrises);
            await db.SaveChangesAsync();
        }


        public async Task<List<Joueur>> GetLastGames(string accountId,string serveur)
        {
            List<Joueur> history = new List<Joueur>();


            string urlMatchList = "https://" + serveur + ".api.riotgames.com/lol/match/v4/matchlists/by-account/" + accountId + "?endIndex=5";
            JObject jsonMatchList = await call.HttpGetJObject(urlMatchList);

            List<Task<JObject>> tasks = new List<Task<JObject>>();
            foreach (JObject match in (JArray)jsonMatchList["matches"])
            {
               
                string urlInfoPartie = "https://" + serveur + ".api.riotgames.com/lol/match/v4/matches/"
                    + match["gameId"].ToString();
                //Info de la partie
                Task<JObject> task = call.HttpGetJObject(urlInfoPartie);
                tasks.Add(task);
            }

            while (tasks.Count > 0)
            {
                Task<JObject> firstFinishedTask = await Task.WhenAny(tasks);
                tasks.Remove(firstFinishedTask);

                JObject matchInfo = await firstFinishedTask;

                Joueur j = await ConvertJsonGame(matchInfo, accountId,serveur, db);

                history.Add(j);
            }

            return history;
        }

        

        public async Task<Joueur> ConvertJsonGame(JObject jsonMatch, string accountId,string serveur, DataContext db)
        {
            Joueur joueur = new Joueur();
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
                j.IdParticipant = Int32.Parse(playersIdentities["participantId"].ToString());

                string invocateurId = playersIdentities["player"]["summonerId"].ToString();

                j.Invocateur = await SetInvocateur(invocateurId,serveur, partie.Joueurs);

                //Récupération des stats
                foreach (JObject playersStats in (JArray)jsonMatch["participants"])
                {
                    int idParticipant = Int32.Parse(playersStats["participantId"].ToString());
                    if (idParticipant == j.IdParticipant)
                    {
                        int idChamp = Int32.Parse(playersStats["championId"].ToString());
                        j.Champion = await db.Champions.FindAsync(idChamp);


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
                                j.Equipements.Add(await db.Equipements.FindAsync(itemId));
                            else
                            {
                                j.Equipements.Add(await db.Equipements.FindAsync(1));
                            }
                        }
                    }
                }

                j.Partie = partie;
                partie.Joueurs.Add(j);

                if (j.Invocateur.AccountId == accountId)
                    joueur = j;

            }

            return joueur;
        }

        public async Task<Invocateur> SetInvocateur(string id,string serveur, List<Joueur> pendingJoueurs)
        {
            Invocateur invocateur;
            if(pendingJoueurs.Where(x => x.Invocateur.Id == id).Count() == 0 && await db.Invocateurs.FindAsync(id) == null)
            {
                invocateur = await GetSummoner(id,serveur, call, db);
                db.Invocateurs.Add(invocateur);
                await db.SaveChangesAsync();
            }
            else if (await db.Invocateurs.FindAsync(id) != null)
            {
                invocateur = await db.Invocateurs.FindAsync(id);
            }
            else
            {
                invocateur = pendingJoueurs.Where(x => x.Invocateur.Id == id).First().Invocateur;
            }

            return invocateur;
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
                        UrlImage = "http://ddragon.leagueoflegends.com/cdn/10.11.1/img/spell/" + x.ElementAt(0)["image"]["full"].ToString(),
                        Joueurs = new List<Joueur>()
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
                        Splash = "http://ddragon.leagueoflegends.com/cdn/img/champion/loading/" + x.ElementAt(0)["image"]["full"].ToString().Split('.')[0] + "_0.jpg",
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
                        Description = x.ElementAt(0)["description"].ToString(),
                        Joueurs = new List<Joueur>()
                    });
                }
            }
            return equipements;
        }

        public async static Task<List<Maitrise>> GetMaitrise(string sumId, DataContext db)
        {
            List<Maitrise> maitrises = new List<Maitrise>();
            HttpClient client = new HttpClient();
            Invocateur i = db.Invocateurs.Find(sumId);
            client.DefaultRequestHeaders.Add("X-Riot-Token", api_key);

            var response = await client.GetAsync("https://" + i.Region.ToLower() + ".api.riotgames.com/lol/champion-mastery/v4/champion-masteries/by-summoner/" + sumId);

            if (response.IsSuccessStatusCode)
            {
                string content = await response.Content.ReadAsStringAsync();
                JArray maitriseJson = JArray.Parse(content);

                foreach (JObject m in maitriseJson)
                {
                    Maitrise maitrise = new Maitrise();
                    //maitrise.Invocateur = await GetSummoner(sumId);
                    maitrise.Niveau = Int32.Parse(m["championLevel"].ToString());
                    maitrise.Champion = db.Champions.Find(Int32.Parse(m["championId"].ToString()));
                    maitrise.ChampionXp = long.Parse(m["championPoints"].ToString());
                    maitrise.XpRestanteProchainNiveau = Int32.Parse(m["championPointsUntilNextLevel"].ToString());

                    maitrises.Add(maitrise);
                }
            }

            return maitrises;
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