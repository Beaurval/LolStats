using PocketSummonner.Models;
using PocketSummonner.Models.BDD;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;

namespace PocketSummonner.Controllers
{
    

    public class LoginController : Controller
    {
        private DataContext db = new DataContext();

        [AllowAnonymous]
        public ActionResult Login()
        {
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

                    // Add the cookie to the response.
                    Response.Cookies.Add(authenticationCookie);



                    return RedirectToAction("Index", new { Controller = "Accueil" });
            }

            //Erreur
            ModelState.AddModelError("IncorrectCreditentials",
                "Les informations d'autenthifications sont invalides");
            return RedirectToAction("Index");
        }

        public ActionResult Inscription ()
        {
            return View();
        }
        [HttpPost]
        public ActionResult Inscription(User u)
        {
            if (ModelState.IsValid)
            {
                db.Users.Add(u);
                db.SaveChanges();
            }

            return View();
        }

        public bool ValidateUser(string fullName, string password)
        {
            return true;
        }
    }
}