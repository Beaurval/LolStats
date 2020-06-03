using Microsoft.Ajax.Utilities;
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
            //await SaveToDb.MajSpells(db);
            //await SaveToDb.MajEquipemment(db);
            //await SaveToDb.MajChampions(db);
            Invocateur invocateur = new Invocateur();
            //Utilisateur en base ?
            if (db.Invocateurs.Where(x => x.Id == summonerId).Count() > 0)
            {
                invocateur = db.Invocateurs.Where(x => x.Id == summonerId).FirstOrDefault();
            }
            //Sinon on récupère l'invocateur et on le sauvegarde en base
            else
            {
                invocateur = await ApiCall.GetSummoner(summonerId);
                db.Invocateurs.Add(invocateur);
                db.SaveChanges();
            }

            //On récupère les derniers joueurs que l'invocateur a incarné
            List<Joueur> lastJoueurs = new List<Joueur>();
            //L'utilisateur a des joueurs en base ?
            if (invocateur.Joueurs.Count > 0)
            {
                lastJoueurs = invocateur.Joueurs;
            }
            //Sinon on les récupères avec l'api et les stock en base
            else
            {
                lastJoueurs = await ApiCall.GetGameHistory(invocateur.AccountId,db);
                db.Joueurs.AddRange(lastJoueurs);
                db.SaveChanges();
            }
                 
            return View(lastJoueurs);
        }

        public ActionResult Recherche(string name)
        {
            ViewBag.name = name;
            return View();
        }

    }
}