using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using GR_Calcul.Models;
using System.Web.Security;
using System.Web.Routing;

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
            HttpContext ctx = HttpContext.Current;
            UrlHelper helper = new UrlHelper(filterContext.RequestContext);
            String url = helper.Action("AccessDenied", "Account");
            if (u != null && u is Person)
            {
                Person p = (Person)u;
                if (!p.IsInRole(_acceptedRoles))
                {
                    ctx.Response.Redirect(url);
                }
                return;
            }
            ctx.Response.Redirect(url);
            
        }

    }

}
