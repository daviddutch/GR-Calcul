using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Security;

namespace GR_Calcul.Models
{
    public class MyProvider : MembershipProvider
    {
        //Minimun password length
        private int minRequiredPasswordLength = 3;
        //Minium non-alphanumeric char required
        private int minRequiredNonAlphanumericCharacters = 0;
        //Enable - disable password retrieval
        private bool enablePasswordRetrieval = true;
        //Enable - disable password reseting
        private bool enablePasswordReset = true;
        //Require security question and answer (this, for instance, is a functionality which not many people use)
        private bool requiresQuestionAndAnswer = false;
        //Application name
        private string applicationName = "GR-Calcul";

        //Require email to be unique 
        private bool requiresUniqueEmail = true;
        //Password format
        private MembershipPasswordFormat passwordFormat = new MembershipPasswordFormat();
        //Regular expression the password should match (empty for none)
        private string passwordStrengthRegularExpression = String.Empty;


        public override bool ValidateUser(string username, string password)
        {
            //For our example, user will be authenticated if username and password are the same
            return username == password;
        }

    }
}