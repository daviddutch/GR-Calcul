using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using GR_Calcul.Models;
using System.Web.Security;

namespace GR_Calcul.Misc
{
    public class DuffAuthorizeAttribute : FilterAttribute, IAuthorizationFilter
    {
        private readonly PersonType[] _acceptedRoles;

        public DuffAuthorizeAttribute(params PersonType[] acceptedRoles)
        {
            _acceptedRoles = acceptedRoles;
        }

        public void OnAuthorization(AuthorizationContext filterContext)
        {
            Exception ex = new Exception("Access denied");
            MembershipUser u = Membership.GetUser(filterContext.HttpContext.User.Identity.Name);
            if (u != null && u is Person)
            {
                Person p = (Person)u;
                if (!p.IsInRole(_acceptedRoles))
                    throw ex;
                return;
            }
            throw ex;
        }

    }

}
