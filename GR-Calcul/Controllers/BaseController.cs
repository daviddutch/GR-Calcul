using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.UI.WebControls;
using System.Web.Security;
using GR_Calcul.Models;


namespace GR_Calcul.Controllers
{
    /// <summary>
    /// Main controller class to generate the menu depending on the user rights
    /// </summary>
    public class BaseController : Controller
    {

        protected override void OnActionExecuted(ActionExecutedContext filterContext)
        {
            // build list of menu items based on user's permissions, and add it to ViewData
            MembershipUser u = Membership.GetUser(filterContext.HttpContext.User.Identity.Name);
            IEnumerable<MyMenuItem> menu = BuildMenu(u);
            ViewData["Menu"] = menu;
        }

        private IEnumerable<MyMenuItem> BuildMenu(MembershipUser u)
        {
            if (u != null && u is Person)
            {
                Person p = (Person)u;
                List<MyMenuItem> list = new List<MyMenuItem>();
                MyMenuItem menu1 = new MyMenuItem("Liste des personnes", "Index", "Person");
                MyMenuItem menu2 = new MyMenuItem("Liste des machines", "List", "Machine");
                MyMenuItem menu3 = new MyMenuItem("Liste des cours", "List", "Course");
                MyMenuItem menu4 = new MyMenuItem("Liste des salles", "Index", "Room");
                MyMenuItem menu5 = new MyMenuItem("Liste de mes cours", "ListMyCourse", "Course");
                switch (p.pType)
                {
                    case PersonType.ResourceManager:
                        list.Add(menu1);
                        list.Add(menu2);
                        list.Add(menu3);
                        list.Add(menu4);
                        break;
                    case PersonType.Responsible:
                        list.Add(menu2);
                        list.Add(menu3);
                        break;
                    case PersonType.User:
                        list.Add(menu3);
                        list.Add(menu5);
                        break;
                }
                
                return list;
            }
            else
            {
                //build default menu, i.e an empty menu
                return new List<MyMenuItem>();
            }
        }

    }
}