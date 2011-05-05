﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using GR_Calcul.Models;
using System.Web.Security;

namespace GR_Calcul.Misc
{
    public class SessionManager
    {

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

    }
}