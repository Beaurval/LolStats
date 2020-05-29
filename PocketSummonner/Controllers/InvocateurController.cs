using Newtonsoft.Json.Linq;
using PocketSummonner.Data;
using PocketSummonner.Helpers;
using PocketSummonner.Models;
using PocketSummonner.Models.BDD;
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
        private DataContext db = new DataContext();

        

        //// GET: Invocateur
        public async Task<ActionResult> Profil(string summonerId)
        {
            Invocateur invocateur = new Invocateur();
            //Utilisateur en base ?
            if (db.Invocateurs.Where(x => x.Id == summonerId).Count() > 0)
            {
                invocateur = db.Invocateurs.Where(x => x.Id == summonerId).FirstOrDefault();
            }
            //Sinon on récupère l'invocateur et on le sauvegarde en base
            else
            {
                invocateur = ConvertJson.ConvertJsonSummoner(await ApiCall.GetJsonSummoner(summonerId));
                db.Invocateurs.Add(invocateur);
                db.SaveChanges();
            }

            //On récupère les parties
            List<Partie> lastMatchs = new List<Partie>();
            //L'utilisateur a des parties en base ?
            if (invocateur.DernieresParties.Count > 0)
            {
                db.Parties.Where(x => x.Joueur.Invocateur.Id == invocateur.Id).ToList();
            }
            //Sinon on les récupères avec l'api et les stock en base
            else
            {
                lastMatchs = await ApiCall.GetGameHistory(invocateur.AccountId);
                db.Parties.AddRange(lastMatchs);
                db.SaveChanges();
            }
                 
            return View(lastMatchs);
        }

        public ActionResult Recherche(string name)
        {
            ViewBag.name = name;
            return View();
        }

    }
}