using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Security;
using GR_Calcul.Models;

public class MyMembershipProvider : MembershipProvider
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

    private PersonModel personModel = new PersonModel();

    public override bool EnablePasswordRetrieval
    {
        get
        {
            return enablePasswordRetrieval;
        }
    }

    public override bool EnablePasswordReset
    {
        get
        {
            return enablePasswordReset;
        }
    }

    public override bool RequiresQuestionAndAnswer
    {
        get
        {
            return requiresQuestionAndAnswer;
        }
    }

    public override string ApplicationName
    {
        get
        {
            return applicationName;
        }
        set
        {
            applicationName = value;
        }
    }

    public override bool ValidateUser(string username, string password)
    {
        Person p = personModel.GetPerson(username, password);
        if (p == null)
        {
            return false;
        }
        HttpContext.Current.Session["username"] = username;
        HttpContext.Current.Session["role"] = p.pType;

        return true;
    }

    public override string GetUserNameByEmail(string email)
    {
        return personModel.GetPersonByEmail(email);
    }

    public bool ChangePassword(string username, string newPwd)
    {
        ValidatePasswordEventArgs args = new ValidatePasswordEventArgs(username, newPwd, true);

        OnValidatingPassword(args);

        if (args.Cancel)
            if (args.FailureInformation != null)
                throw args.FailureInformation;
            else
                throw new MembershipPasswordException("Change password canceled due to new password validation failure.");
        int affectesRows = personModel.ChangePassword(username, newPwd);
        return affectesRows > 0;
    }

    public override bool ChangePassword(string username, string oldPwd, string newPwd)
    {
        if (!ValidateUser(username, oldPwd))
            return false;

        return ChangePassword(username, newPwd);
    }

    public override bool ChangePasswordQuestionAndAnswer(string username, string password, string newPasswordQuestion, string newPasswordAnswer)
    {        
        throw new NotImplementedException();
    }

    public override MembershipUser CreateUser(string username, string password, string email, string passwordQuestion, string passwordAnswer, bool isApproved, object providerUserKey, out MembershipCreateStatus status)
    {
        throw new NotImplementedException();
    }

    public override bool DeleteUser(string username, bool deleteAllRelatedData)
    {
        throw new NotImplementedException();
    }

    public override MembershipUserCollection FindUsersByEmail(string emailToMatch, int pageIndex, int pageSize, out int totalRecords)
    {
        throw new NotImplementedException();
    }

    public override MembershipUserCollection FindUsersByName(string usernameToMatch, int pageIndex, int pageSize, out int totalRecords)
    {
        throw new NotImplementedException();
    }

    public override MembershipUserCollection GetAllUsers(int pageIndex, int pageSize, out int totalRecords)
    {
        List<Person> list = personModel.ListPerson();
        totalRecords = list.Count;
        MembershipUserCollection ret = new MembershipUserCollection();
        foreach (Person p in list)
            ret.Add(p);
        return ret;
    }

    public override int GetNumberOfUsersOnline()
    {
        throw new NotImplementedException();
    }

    public override string GetPassword(string username, string answer)
    {
        Person p = personModel.GetPerson(username);
        if(p != null)
            return p.Password;
        return null;
    }

    public override MembershipUser GetUser(string username, bool userIsOnline)
    {
        return personModel.GetPerson(username);
    }

    public override MembershipUser GetUser(object providerUserKey, bool userIsOnline)
    {
        throw new NotImplementedException();
    }

    public override int MinRequiredNonAlphanumericCharacters
    {
        get { return minRequiredNonAlphanumericCharacters; }
    }

    public override int MinRequiredPasswordLength
    {
        get { return minRequiredPasswordLength; }
    }

    public override int PasswordAttemptWindow
    {
        get { throw new NotImplementedException(); }
    }

    public override MembershipPasswordFormat PasswordFormat
    {
        get { return passwordFormat; }
    }

    public override string PasswordStrengthRegularExpression
    {
        get { return passwordStrengthRegularExpression; }
    }

    public override bool RequiresUniqueEmail
    {
        get { return requiresUniqueEmail; }
    }

    public override int MaxInvalidPasswordAttempts
    {
        get { throw new NotImplementedException(); }
    }

    public override string ResetPassword(string username, string answer)
    {
        throw new NotImplementedException();
    }

    public override bool UnlockUser(string userName)
    {
        throw new NotImplementedException();
    }

    public override void UpdateUser(MembershipUser user)
    {
        throw new NotImplementedException();
    }
}
