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
    public static class IdToName
    {
        private static string api_key = Properties.Settings.Default.api_key;
        public async static Task<string> GetChampionName(int idChampion)
        {
            string champName = "Aatrox";
            HttpClient client = new HttpClient();
            client.DefaultRequestHeaders.Add("X-Riot-Token", api_key);

            var response = await client.GetAsync("http://ddragon.leagueoflegends.com/cdn/10.6.1/data/en_US/champion.json");
            string responseString = await response.Content.ReadAsStringAsync();

            if (response.IsSuccessStatusCode)
            {
                JObject jsonChampions = JObject.Parse(responseString);
                JObject dataChampions = (JObject)jsonChampions["data"];
                foreach (JProperty prop in dataChampions.Properties())
                {

                    if (Int32.Parse(prop.Value["key"].ToString()) == idChampion)
                    {
                        champName = prop.Name;
                    }
                }
            }

            return champName;
        }

        public async static Task<string> GetSpellName(int idSpell)
        {
            string spellName = "SummonerFlash";
            HttpClient client = new HttpClient();
            client.DefaultRequestHeaders.Add("X-Riot-Token", api_key);

            var response = await client.GetAsync("http://ddragon.leagueoflegends.com/cdn/10.6.1/data/en_US/summoner.json");
            string responseString = await response.Content.ReadAsStringAsync();

            if (response.IsSuccessStatusCode)
            {
                JObject jsonSpells = JObject.Parse(responseString);
                JObject dataSpells = (JObject)jsonSpells["data"];
                foreach (JProperty prop in dataSpells.Properties())
                {

                    if (Int32.Parse(prop.Value["key"].ToString()) == idSpell)
                    {
                        spellName = prop.Name;
                    }
                }
            }

            return spellName;
        }

        public async static Task<Equipement> GetItem(int? idItem)
        {
            Equipement item = new Equipement();
            item.Image = "https://upload.wikimedia.org/wikipedia/commons/5/5f/Grey.PNG";
            if (idItem.HasValue && idItem != 0)
            {
                HttpClient client = new HttpClient();
                client.DefaultRequestHeaders.Add("X-Riot-Token", api_key);

                var response = await client.GetAsync("http://ddragon.leagueoflegends.com/cdn/10.6.1/data/en_US/item.json");
                string responseString = await response.Content.ReadAsStringAsync();

                if (response.IsSuccessStatusCode)
                {
                    JObject jsonItems = JObject.Parse(responseString);
                    JObject dataItems = (JObject)jsonItems["data"];
                    foreach (JProperty prop in dataItems.Properties())
                    {
                        if (prop.Name == idItem.ToString())
                        {
                            item.Description = prop.Value["description"].ToString();
                            item.Image = "http://ddragon.leagueoflegends.com/cdn/10.6.1/img/item/" + idItem.ToString() + ".png";
                            item.Nom = prop.Value["name"].ToString();
                        }
                    }
                }
                else
                {
                    item.Image = "https://upload.wikimedia.org/wikipedia/commons/5/5f/Grey.PNG";
                    item.Nom = "";
                    item.Description = "";
                }

            }
            else
            {

                item.Image = "https://upload.wikimedia.org/wikipedia/commons/5/5f/Grey.PNG";
                item.Nom = "";
                item.Description = "";
            }

            return item;
        }
    }
}