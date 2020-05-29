using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Web;
using PocketSummonner.Models;
using MySql.Data.MySqlClient;

namespace PocketSummonner.Data
{
    public class Api
    {
        public JToken champions;
        public Api()
        {
            champions = JObject.Parse(new WebClient().DownloadString("http://ddragon.leagueoflegends.com/cdn/10.5.1/data/fr_FR/champion.json"))["data"];
        }

        public string GetName(int idChampion)
        {
            string name = "";
            try
            {
                MySqlConnection con = new MySqlConnection("Persist Security Info=False;database=pocketsummoner;server=localhost;user id=user;Password=resu");
                con.Open();


                MySqlCommand cmd = new MySqlCommand();
                cmd.Connection = con;
                cmd.CommandText = "SELECT name FROM champions WHERE id = ?id";
                cmd.Parameters.Add("?id", MySqlDbType.VarChar).Value = idChampion;

                MySqlDataReader rdr = cmd.ExecuteReader();
                while (rdr.Read())
                {
                    name = rdr[0].ToString();
                }

                con.Close();
            }
            catch
            {

            }

            return name;
        }


        public void MajChampions()
        {
            try
            {
                MySqlConnection con = new MySqlConnection("Persist Security Info=False;database=pocketsummoner;server=localhost;user id=user;Password=resu");
                con.Open();

                foreach (JProperty tk in champions)
                {
                    MySqlCommand cmd = new MySqlCommand();
                    cmd.Connection = con;
                    cmd.CommandText = "INSERT INTO champions VALUES(?id,?name)";
                    cmd.Parameters.Add("?id", MySqlDbType.VarChar).Value = tk.Value["key"];
                    cmd.Parameters.Add("?name", MySqlDbType.VarChar).Value = tk.Value["id"];
                    cmd.ExecuteNonQuery();
                }
                con.Close();

            }
            catch
            {

            }
        }
    }
}