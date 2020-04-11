using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace PocketSummonner.Controllers
{
    public class AccueilController : Controller
    {
        public ActionResult Index()
        {
            return View();
        }
    }
}