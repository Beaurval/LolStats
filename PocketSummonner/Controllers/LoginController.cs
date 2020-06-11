using Newtonsoft.Json.Linq;
using PocketSummonner.Helpers;
using PocketSummonner.Models;
using PocketSummonner.Models.BDD;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;

namespace PocketSummonner.Controllers
{
    public class LoginController : Controller
    {
        private DataContext db = new DataContext();
        private String[] Servers;


        public async Task<string> GetInvocateurIdByName(string name, string serveur)
        {
            HttpCall call = new HttpCall();
            JObject summJson = await call.HttpGetJObject("https://" + serveur + ".api.riotgames.com/lol/summoner/v4/summoners/by-name/" + name);

            return summJson["id"].ToString();
        }

        public LoginController()
        {
            Servers = new string[11]{
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

        [AllowAnonymous]
        public ActionResult Login()
        {
            if (User.Identity.IsAuthenticated)
                return RedirectToAction("Index", "Accueil");

            return View();
        }

        // GET: Login
        [AllowAnonymous]
        [HttpPost]
        public ActionResult Login(UserModel user, bool? rememberMe)
        {
            //Authentification réussie
            if (ValidateUser(user.Mail, user.Password))
            {
                // Clear any other tickets that are already in the response
                Response.Cookies.Clear();

                // Set the new expiry date - to thirty days from now
                DateTime expiryDate = DateTime.Now.AddDays(30);

                // Create a new forms auth ticket
                FormsAuthenticationTicket ticket = new FormsAuthenticationTicket(2, user.Mail, DateTime.Now, expiryDate, true, String.Empty);

                // Encrypt the ticket
                string encryptedTicket = FormsAuthentication.Encrypt(ticket);

                // Create a new authentication cookie - and set its expiration date
                HttpCookie authenticationCookie = new HttpCookie(FormsAuthentication.FormsCookieName, encryptedTicket);
                authenticationCookie.Expires = ticket.Expiration;

                HttpCookie cookie = new HttpCookie("UserId");
                try
                {
                    cookie.Value = db.Users.Where(x => x.Mail == user.Mail).FirstOrDefault().Id.ToString();
                    //cookie.Value = "http://ddragon.leagueoflegends.com/cdn/10.11.1/img/profileicon/" + db.Users.Where(x => x.Mail == user.Mail).FirstOrDefault().invocateur.ImageProfil + ".png";
                    Response.Cookies.Add(cookie);
                }
                catch
                {

                }
                // Add the cookie to the response.
                Response.Cookies.Add(authenticationCookie);

                return RedirectToAction("Index", new { Controller = "Accueil" });
            }

            //Erreur
            ModelState.AddModelError("IncorrectCreditentials",
                "Les informations d'autenthifications sont invalides");
            return RedirectToAction("Login");
        }

        public ActionResult Inscription()
        {
            ViewBag.Servers = Servers;
            return View();
        }
        [HttpPost]
        public async Task<ActionResult> Inscription(User u, string summName, string serveur)
        {
            if (User.Identity.IsAuthenticated)
                return RedirectToAction("Index", "Accueil");

            ViewBag.Servers = Servers;
            string invocateurID = await GetInvocateurIdByName(summName, serveur);
            Invocateur invocateur;
            if (await db.Invocateurs.FindAsync(invocateurID) != null)
            {
                invocateur = await db.Invocateurs.FindAsync(invocateurID);
                u.invocateur = invocateur;
            }
            else
            {
                invocateur = await ApiCall.GetSummoner(invocateurID,serveur, new HttpCall(), db);
                u.invocateur = invocateur;
                db.Invocateurs.Add(invocateur);
                await db.SaveChangesAsync();
            }

            if (ModelState.IsValid)
            {
                db.Users.Add(u);
                db.SaveChanges();
            }
            else
            {
                return View();
            }

            return RedirectToAction("Login");
        }

        public bool ValidateUser(string mail, string password)
        {
            if (db.Users.Where(x => x.Mail == mail).Count() > 0)
            {
                if (db.Users.Where(x => x.Mail == mail).First().Password == password)
                    return true;
                else
                    return false;
            }
            else
                return false;
        }

        public ActionResult Logout()
        {
            FormsAuthentication.SignOut();
            return RedirectToAction("Index", "Accueil");
        }
    }
}