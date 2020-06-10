using PocketSummonner.Models;
using PocketSummonner.Models.BDD;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace PocketSummonner.Controllers
{
    [Authorize]
    public class AccueilController : Controller
    {
        DataContext db = new DataContext();
        public ActionResult Index()
        {
            List<Invocateur> last = db.Invocateurs
                .OrderByDescending(x => x.DateAjout).Take(5).ToList();
            List<Invocateur> masteries = db.Invocateurs.Where(m => m.Maitrises.Sum(n => n.Niveau) > 0)
                .OrderByDescending(x => x.Maitrises.Sum(n => n.Niveau)).Take(5).ToList();

            AccueilModel model = new AccueilModel
            {
                lastInvocateeur = last,
                topMasteries = masteries
            };
            return View(model);
        }
    }
}