using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

/// <summary>
/// Namespace containing all controllers
/// </summary>
namespace GR_Calcul.Controllers
{
    /// <summary>
    /// Class containing all actions related to the home page module
    /// </summary>
    public class HomeController : BaseController
    {
        public ActionResult Index()
        {
            ViewBag.Message = "Bienvenue sur le site de GR-Calcul";

            return View();
        }
    }
}
