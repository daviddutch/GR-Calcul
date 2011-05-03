using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace GR_Calcul.Misc
{
    public enum UserRole { User, Responsible, ResourceManager };

    public class DuffAuthorizeAttribute : FilterAttribute, IAuthorizationFilter
    {
        public DuffAuthorizeAttribute(params UserRole[] acceptedRoles)
        {
            _acceptedRoles = acceptedRoles;
        }

        public void OnAuthorization(AuthorizationContext filterContext)
        {
            GR_Calcul.Controllers.SlotRangeController.User currentUser = GR_Calcul.Controllers.SlotRangeController.User.GetCurrentUser;

            if (!currentUser.IsInRole(_acceptedRoles))
                throw new Exception("Access denied");
        }

        private readonly UserRole[] _acceptedRoles;

    }

}
