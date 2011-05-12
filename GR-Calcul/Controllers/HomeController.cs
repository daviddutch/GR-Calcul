using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace GR_Calcul.Controllers
{
    public class HomeController : BaseController
    {
        public ActionResult Index()
        {
            ViewBag.Message = "Bienvenue sur le site de GR-Calcul";

            return View();
        }
    }
}
