using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using GR_Calcul.Models;
using System.Web.Security;
using System.Web.Mvc;
using System.Web.Routing;

namespace GR_Calcul.Misc
{
    public class SessionManager
    {

        public static void RedirectAccessDenied(RequestContext context)
        {
            throw new AccessDeniedException();
        }

        public static PersonType? GetCurrentUserRole(string username)
        {
            Person p = GetCurrentUser(username);
            if (p == null)
                return null;
            return p.pType;
        }

        public static int? GetCurrentUserId(string username)
        {
            Person p = GetCurrentUser(username);
            if (p == null)
                return null;
            return p.ID;
        }

        public static Person GetCurrentUser(string username)
        {
            MembershipUser u = Membership.GetUser(username);
            if (u != null && u is Person)
            {
                Person p = (Person)u;
                return p;
            }
            return null;
        }

        public static Boolean IsLogged(HttpContextBase context)
        {
            return !String.IsNullOrEmpty(context.User.Identity.Name);
        }

    }
}