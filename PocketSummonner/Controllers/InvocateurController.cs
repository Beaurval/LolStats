﻿using Microsoft.Ajax.Utilities;
using Newtonsoft.Json.Linq;
using PocketSummonner.Helpers;
using PocketSummonner.Models;
using PocketSummonner.Models.BDD;
using PocketSummonner.Models.Profil;
using RestSharp;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing.Drawing2D;
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
        public ApiCall api;
        public SaveToDb stdb;
        private HttpCall call;
        private String[] serveurs;

        public InvocateurController()
        {
            stdb = new SaveToDb(db);
            call = new HttpCall();
            api = new ApiCall(db, call);

            serveurs = new string[11]{
                "br1",
                "eun1",
                "euw1",
                "jp1",
                "kr",
                "la1",
                "la2",
                "na1",
                "oc1",
                "tr1",
                "ru"
            };
        }

        //// GET: Invocateur
        public async Task<ActionResult> Profil(string summonerId, string serveur, bool update = false)
        {
            

            Invocateur invocateur = new Invocateur();
            List<Maitrise> maitrises = new List<Maitrise>();

            if (await db.Invocateurs.FindAsync(summonerId) == null)
            {
                invocateur = await ApiCall.GetSummoner(summonerId,serveur, call, db);
                db.Invocateurs.Add(invocateur);
            }
            else
            {
                invocateur = await db.Invocateurs.FindAsync(summonerId);
                await db.SaveChangesAsync();
            }


            Fonctions utils = new Fonctions(this.Request);
            int userId = utils.CurrentUser().Id;
            string userInvocateurId = utils.CurrentUser().invocateur.Id;

            if (db.Vues.Where(x => x.UserId == userId && x.InvocateurId == summonerId).Count() == 0 && userInvocateurId != summonerId)
                db.Vues.Add(new UserSawInvocateur { InvocateurId = summonerId, UserId = userId, DateConsultation = DateTime.Now });
            else if (userInvocateurId != summonerId)
                db.Vues.Where(x => x.UserId == userId && x.InvocateurId == summonerId).FirstOrDefault().DateConsultation = DateTime.Now;

            await db.SaveChangesAsync();

            if (invocateur.Maitrises != null)
                maitrises = invocateur.Maitrises;
            //On récupère les derniers joueurs que l'invocateur a incarné
            List<Joueur> lastJoueurs = new List<Joueur>();
            //L'utilisateur a des joueurs en base ?
            if (invocateur.Joueurs.Count > 0 && !update)
            {
                lastJoueurs = invocateur.Joueurs;
            }
            //Sinon on les récupères avec l'api et les stock en base
            else
            {
                
                List<Joueur> jrs = await api.GetLastGames(invocateur.AccountId,serveur);
                db.Joueurs.AddRange(jrs);
                await db.SaveChangesAsync();
                lastJoueurs = jrs;
            }

            return View(new ProfilModel { DernieresParties = lastJoueurs, Maitrises = maitrises, History = db.Vues.Where(x => x.UserId == userId).OrderByDescending(x => x.DateConsultation).ToList() });
        }

        public async Task<ActionResult> Update(string summId)
        {
            ViewBag.SummId = summId;
            await ApiCall.MajMaitrises(summId, db);
            return RedirectToAction("Profil",new { summonerId = summId });
        }


        public  ActionResult Recherche(string name)
        {
            ViewBag.name = name;
            return View();
        }

    }
}