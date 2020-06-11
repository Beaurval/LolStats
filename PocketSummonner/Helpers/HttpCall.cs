using Newtonsoft.Json.Linq;
using RestSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web;

namespace PocketSummonner.Helpers
{
    public class HttpCall
    {
        private string api_key = Properties.Settings.Default.api_key;
        private int nbRequetes;

        public HttpCall()
        {
            nbRequetes = 0;
        }

        public async Task<JObject> HttpGetJObject(string url)
        {
            System.Diagnostics.Debug.WriteLine(url);

            string content = "";
            HttpClient client = new HttpClient();
            client.DefaultRequestHeaders.Add("X-Riot-Token", api_key);

            HttpResponseMessage response;

            do
            {
                response = await client.GetAsync(url);
                if (!response.IsSuccessStatusCode)
                {
                    System.Threading.Thread.Sleep(500);
                    System.Diagnostics.Debug.WriteLine("Trop de requêtes, on attend ...");
                }
                else
                    content = await response.Content.ReadAsStringAsync();
            } while (!response.IsSuccessStatusCode);


            nbRequetes++;
            return JObject.Parse(content);
        }

        public async Task<JArray> HttpGetJArray(string url)
        {


            System.Diagnostics.Debug.WriteLine(url);
            string content = "";

            HttpClient client = new HttpClient();
            client.DefaultRequestHeaders.Add("X-Riot-Token", api_key);

            HttpResponseMessage response;
            do
            {
                response = await client.GetAsync(url);

                if (!response.IsSuccessStatusCode)
                {
                    System.Diagnostics.Debug.WriteLine("Trop de requêtes, on attend ..."); System.Threading.Thread.Sleep(500);
                }
                else
                    content = await response.Content.ReadAsStringAsync();
            } while (!response.IsSuccessStatusCode);


            nbRequetes++;
            return JArray.Parse(content);
        }
    }
}