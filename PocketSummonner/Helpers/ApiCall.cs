using Newtonsoft.Json.Linq;
using PocketSummonner.Models.BDD;
using PocketSummonner.Models.Profil;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web;
using System.Web.UI.WebControls.WebParts;

namespace PocketSummonner.Helpers
{
    public static class ApiCall
    {
        private static string api_key = Properties.Settings.Default.api_key;
        public async static Task<JObject> GetJsonSummoner(string summonerId)
        {
            HttpClient client = new HttpClient();
            client.DefaultRequestHeaders.Add("X-Riot-Token", api_key);

            var response = await client.GetAsync("https://euw1.api.riotgames.com/lol/summoner/v4/summoners/" + summonerId);

            if (response.IsSuccessStatusCode)
            {
                string content = await response.Content.ReadAsStringAsync();
                JObject jsonInvocateur = JObject.Parse(content);

                return jsonInvocateur;
            }
            else
                return new JObject();
        }

        public async static Task<List<Partie>> GetGameHistory(string accountId)
        {
            List<Partie> history = new List<Partie>();
            HttpClient client = new HttpClient();
            client.DefaultRequestHeaders.Add("X-Riot-Token", api_key);

            var response = await client.GetAsync("https://euw1.api.riotgames.com/lol/match/v4/matchlists/by-account/" + accountId + "?endIndex=10");

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
                        partie.AutreJoueurs = new List<Joueur>();

                        //Joueur du profil
                        partie.Joueur = new Joueur();
                        partie.Joueur.Equipements = new List<Equipement>();

                        partie.DatePartie = UnixTimeStampToDateTime(Double.Parse(jsonMatch["gameCreation"].ToString()) / 1000);
                        partie.Duree = Int32.Parse(jsonMatch["gameDuration"].ToString());
                        partie.TypePartie = jsonMatch["gameMode"].ToString();

                        //Récupération des identités des joueurs de la game
                        foreach (JObject playersIdentities in (JArray)jsonMatch["participantIdentities"])
                        {
                            Joueur j = new Joueur();
                            Invocateur invocateurJoueur
                                = new Invocateur
                                {
                                    Id = playersIdentities["player"]["summonerId"].ToString(),
                                    Name = playersIdentities["player"]["summonerName"].ToString(),
                                    AccountId = playersIdentities["player"]["accountId"].ToString(),
                                };

                            j.IdParticipant = Int32.Parse(playersIdentities["participantId"].ToString());
                            j.Invocateur = invocateurJoueur;

                            if (j.Invocateur.AccountId == accountId)
                                partie.Joueur = j;
                            partie.AutreJoueurs.Add(j);
                        }

                        //Récupération des stats invocateur principal et teams
                        foreach (JObject playersStats in (JArray)jsonMatch["participants"])
                        {
                            int idParticipant = Int32.Parse(playersStats["participantId"].ToString());
                            string nomChampion = await Helpers.IdToName.GetChampionName(Int32.Parse(playersStats["championId"].ToString()));
                            Champion champion = new Champion
                            {
                                Id = Int32.Parse(playersStats["championId"].ToString()),
                                Nom = nomChampion,
                                Image = "http://ddragon.leagueoflegends.com/cdn/10.6.1/img/champion/"
                                + nomChampion + ".png"
                            };

                            //Récup des infos du participant
                            Joueur j = partie.AutreJoueurs.Where(x => x.IdParticipant == idParticipant).First();
                            j.Equipe = new Equipe { Id = Int32.Parse(playersStats["teamId"].ToString()) };
                            j.Champion = champion;

                            //Récup info invocateur pricipal
                            if (idParticipant == partie.Joueur.Id)
                            {
                                //Victoire ?
                                partie.Victoire = bool.Parse(playersStats["stats"]["win"].ToString());
                                //Spells invocateur
                                partie.Joueur.Sort1 = "http://ddragon.leagueoflegends.com/cdn/10.6.1/img/spell/" +
                                    await Helpers.IdToName.GetSpellName(Int32.Parse(playersStats["spell1Id"].ToString()))
                                    + ".png";
                                partie.Joueur.Sort2 = "http://ddragon.leagueoflegends.com/cdn/10.6.1/img/spell/" +
                                    await Helpers.IdToName.GetSpellName(Int32.Parse(playersStats["spell2Id"].ToString()))
                                    + ".png";

                                //Img du champion
                                partie.Joueur.NbTue = Int32.Parse(playersStats["stats"]["kills"].ToString());
                                partie.Joueur.NbMort = Int32.Parse(playersStats["stats"]["deaths"].ToString());
                                partie.Joueur.NbAssist = Int32.Parse(playersStats["stats"]["assists"].ToString());
                                partie.Joueur.NbSbire = Int32.Parse(playersStats["stats"]["totalMinionsKilled"].ToString());
                                partie.Joueur.Level = Int32.Parse(playersStats["stats"]["champLevel"].ToString());
                                partie.Joueur.KDA = (partie.Joueur.NbTue + partie.Joueur.NbAssist) / partie.Joueur.NbMort;
                                partie.Joueur.Poste = playersStats["timeline"]["lane"].ToString();

                                for (int i = 0; i < 6; i++)
                                {
                                    string path = VirtualPathUtility.ToAbsolute("~/Content/img/assets/empty.png");

                                    Equipement item = await Helpers.IdToName.GetItem(Int32.Parse(playersStats["stats"]["item" + i].ToString()));
                                    partie.Joueur.Equipements.Add(item);
                                }
                            }

                        }
                        history.Add(partie);
                    }

                }
            }
            return history;
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