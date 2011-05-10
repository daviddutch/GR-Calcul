using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using GR_Calcul.Models;
using System.Configuration;

namespace GR_Calcul.Misc
{
    public class ConnectionManager
    {
        //static private ConnectionStringSettings config = System.Configuration.ConfigurationManager.ConnectionStrings["DB"];
        public static string GetConnectionString()
        {
            //return System.Configuration.ConfigurationManager.ConnectionStrings["LocalDB"].ConnectionString;
            switch (SessionManager.GetCurrentUserRole(System.Web.HttpContext.Current.User.Identity.Name))
            {
                case PersonType.User:
                    return System.Configuration.ConfigurationManager.ConnectionStrings["DB_User"].ConnectionString;
                case PersonType.Responsible:
                    return System.Configuration.ConfigurationManager.ConnectionStrings["DB_Responsible"].ConnectionString;
                case PersonType.ResourceManager:
                    return System.Configuration.ConfigurationManager.ConnectionStrings["DB_RM"].ConnectionString;
                default:
                    return GetUnauthentifiedConnectionString();
            }
        }

        public static string GetUnauthentifiedConnectionString()
        {
            return System.Configuration.ConfigurationManager.ConnectionStrings["DB_Unauthentified"].ConnectionString;
        }

    }
}