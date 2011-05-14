using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Globalization;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;
using System.Data.SqlClient;
using System.Data;
using GR_Calcul.Misc;


namespace GR_Calcul.Models
{
    #region Models

    public class ChangePasswordModel
    {
        [Required]
        [DataType(DataType.Password)]
        [Display(Name = "Mot de passe actuel")]
        public string OldPassword { get; set; }

        [Required]
        [ValidatePasswordLength]
        [DataType(DataType.Password)]
        [Display(Name = "Nouveau mot de passe")]
        public string NewPassword { get; set; }

        [DataType(DataType.Password)]
        [Display(Name = "Confirmer le mot de passe")]
        [Compare("NewPassword", ErrorMessage = "Le nouveau mot de passe et la confirmation du nouveau mot de passe ne correspondent pas.")]
        public string ConfirmPassword { get; set; }
    }

    public class LostPasswordChangeModel
    {
        [Required]
        [ValidatePasswordLength]
        [DataType(DataType.Password)]
        [Display(Name = "Nouveau mot de passe")]
        public string NewPassword { get; set; }

        [DataType(DataType.Password)]
        [Display(Name = "Confirmer le mot de passe")]
        [Compare("NewPassword", ErrorMessage = "Le nouveau mot de passe et la confirmation du nouveau mot de passe ne correspondent pas.")]
        public string ConfirmPassword { get; set; }

        public string GetUserFromToken(string token)
        {
            string username = null;
            try
            {
                SqlConnection db = new SqlConnection(ConnectionManager.GetConnectionString()/*System.Configuration.ConfigurationManager.ConnectionStrings["LocalDB"].ConnectionString*/);
                SqlTransaction transaction;
                db.Open();

                transaction = db.BeginTransaction(IsolationLevel.ReadUncommitted);
                try
                {

                    //test whether there's already a pending request
                    SqlCommand cmd = new SqlCommand("SELECT username FROM LostPassword WHERE token=@token; ", db, transaction);

                    cmd.Parameters.Add("@token", SqlDbType.Char);
                    cmd.Parameters["@token"].Value = token;

                    SqlDataReader rdr = cmd.ExecuteReader();

                    if (rdr.Read())
                    {
                        username = rdr.GetString(rdr.GetOrdinal("username"));
                    }
                    rdr.Close();

                    transaction.Commit();
                }
                catch
                {
                    transaction.Rollback();
                }
                finally
                {
                    db.Close();
                }
            }
            catch
            {
            }
            return username;
        }

        public bool IsTokenValid(string token)
        {
            bool valid = false;
            try
            {
                SqlConnection db = new SqlConnection(ConnectionManager.GetConnectionString() /*System.Configuration.ConfigurationManager.ConnectionStrings["LocalDB"].ConnectionString*/);
                SqlTransaction transaction;
                db.Open();

                transaction = db.BeginTransaction(IsolationLevel.ReadUncommitted);
                try
                {

                    //test whether there's already a pending request
                    SqlCommand cmd = new SqlCommand("SELECT token, DATEDIFF(hh, DATEADD(hh, 2, requestDate), GETDATE()) as diff " +
                        "FROM LostPassword WHERE token=@token; ", db, transaction);

                    cmd.Parameters.Add("@token", SqlDbType.Char);
                    cmd.Parameters["@token"].Value = token;

                    SqlDataReader rdr = cmd.ExecuteReader();

                    if (rdr.Read())
                    {
                        int diff = rdr.GetInt32(rdr.GetOrdinal("diff"));
                        valid = diff <= 0;
                    }
                    rdr.Close();

                    transaction.Commit();
                }
                catch
                {
                    transaction.Rollback();
                }
                finally
                {
                    db.Close();
                }
            }
            catch
            {
            }
            return valid;
        }

    }


    public class LogOnModel
    {
        [Required(ErrorMessage = "Le nom d'utilisateur doit être saisi.")]
        [Display(Name = "Nom d'utilisateur")]
        public string UserName { get; set; }

        [Required(ErrorMessage = "Le mot de passe doit être saisi.")]
        [DataType(DataType.Password)]
        [Display(Name = "Mot de passe")]
        public string Password { get; set; }

        [Display(Name = "Enregistrer sur ce PC ?")]
        public bool RememberMe { get; set; }
    }

    public class LostPasswordModel
    {
        [Required(ErrorMessage = "L'adresse email doit être saisie.")]
        [Display(Name = "Adresse email")]
        public string Email { get; set; }

        public void InsertToken(string token, string email)
        {
            string username = Membership.GetUserNameByEmail(email);
            if (username == null)
            {
                throw new NoNullAllowedException();
            }

			SqlConnection db = new SqlConnection(ConnectionManager.GetConnectionString() /*System.Configuration.ConfigurationManager.ConnectionStrings["LocalDB"].ConnectionString*/);
            SqlTransaction transaction;
            db.Open();

            transaction = db.BeginTransaction(IsolationLevel.ReadUncommitted);
            try
            {
                    
                SqlCommand cmd = new SqlCommand("DELETE FROM LostPassword WHERE username=@name; ", db, transaction);
                cmd.Parameters.Add("@name", SqlDbType.Char);
                cmd.Parameters["@name"].Value = username;
                cmd.ExecuteNonQuery();

                cmd = new SqlCommand("INSERT INTO LostPassword (username, token, requestDate) "+
                    "VALUES (@name, @token, GETDATE());", db, transaction);

                cmd.Parameters.Add("@name", SqlDbType.Char);
                cmd.Parameters.Add("@token", SqlDbType.Char);

                cmd.Parameters["@name"].Value = username;
                cmd.Parameters["@token"].Value = token;

                cmd.ExecuteNonQuery();

                transaction.Commit();
            }
            catch(Exception ex)
            {
                transaction.Rollback();
				throw ex;
            }
            finally
            {
                db.Close();
            }
		}
    }

    #endregion

    #region Services
    // The FormsAuthentication type is sealed and contains static members, so it is difficult to
    // unit test code that calls its members. The interface and helper class below demonstrate
    // how to create an abstract wrapper around such a type in order to make the AccountController
    // code unit testable.

    public interface IMembershipService
    {
        int MinPasswordLength { get; }

        bool ValidateUser(string userName, string password);
        MembershipCreateStatus CreateUser(string userName, string password, string email);
        bool ChangePassword(string userName, string oldPassword, string newPassword);
        bool ChangePassword(string token, string newPassword);
    }

    public class AccountMembershipService : IMembershipService
    {
        private readonly MembershipProvider _provider;

        private LostPasswordChangeModel model = new LostPasswordChangeModel();

        public AccountMembershipService()
            : this(null)
        {
        }

        public AccountMembershipService(MembershipProvider provider)
        {
            _provider = provider ?? Membership.Provider;
        }

        public int MinPasswordLength
        {
            get
            {
                return _provider.MinRequiredPasswordLength;
            }
        }

        public bool ValidateUser(string userName, string password)
        {
            if (String.IsNullOrEmpty(userName)) throw new ArgumentException("Value cannot be null or empty.", "userName");
            if (String.IsNullOrEmpty(password)) throw new ArgumentException("Value cannot be null or empty.", "password");

            return _provider.ValidateUser(userName, password);
        }

        public MembershipCreateStatus CreateUser(string userName, string password, string email)
        {
            if (String.IsNullOrEmpty(userName)) throw new ArgumentException("Value cannot be null or empty.", "userName");
            if (String.IsNullOrEmpty(password)) throw new ArgumentException("Value cannot be null or empty.", "password");
            if (String.IsNullOrEmpty(email)) throw new ArgumentException("Value cannot be null or empty.", "email");

            MembershipCreateStatus status;
            _provider.CreateUser(userName, password, email, null, null, true, null, out status);
            return status;
        }

        public bool ChangePassword(string userName, string oldPassword, string newPassword)
        {
            if (String.IsNullOrEmpty(userName)) throw new ArgumentException("Value cannot be null or empty.", "userName");
            if (String.IsNullOrEmpty(oldPassword)) throw new ArgumentException("Value cannot be null or empty.", "oldPassword");
            if (String.IsNullOrEmpty(newPassword)) throw new ArgumentException("Value cannot be null or empty.", "newPassword");

            // The underlying ChangePassword() will throw an exception rather
            // than return false in certain failure scenarios.
            try
            {
                if (_provider.GetUser(userName, true /* userIsOnline */) != null)
                {
                    return _provider.ChangePassword(userName, oldPassword, newPassword);
                }
                return false;
                //return currentUser.ChangePassword(oldPassword, newPassword);
            }
            catch (ArgumentException)
            {
                return false;
            }
            catch (MembershipPasswordException)
            {
                return false;
            }
        }

        public bool ChangePassword(string token, string newPassword)
        {
            if (String.IsNullOrEmpty(token)) throw new ArgumentException("Value cannot be null or empty.", "token");
            if (String.IsNullOrEmpty(newPassword)) throw new ArgumentException("Value cannot be null or empty.", "newPassword");

            // The underlying ChangePassword() will throw an exception rather
            // than return false in certain failure scenarios.
            try
            {
                string username = model.GetUserFromToken(token);
                if (username != null && _provider is MyMembershipProvider)
                {
                    return ((MyMembershipProvider)_provider).ChangePassword(username, newPassword);
                }
                return false;
            }
            catch (ArgumentException)
            {
                return false;
            }
            catch (MembershipPasswordException)
            {
                return false;
            }
        }

    }

    public interface IFormsAuthenticationService
    {
        void SignIn(string userName, bool createPersistentCookie);
        void SignOut();
    }

    public class FormsAuthenticationService : IFormsAuthenticationService
    {
        public void SignIn(string userName, bool createPersistentCookie)
        {
            if (String.IsNullOrEmpty(userName)) throw new ArgumentException("Value cannot be null or empty.", "userName");

            FormsAuthentication.SetAuthCookie(userName, createPersistentCookie);
        }

        public void SignOut()
        {
            FormsAuthentication.SignOut();
        }
    }
    #endregion

    #region Validation
    public static class AccountValidation
    {
        public static string ErrorCodeToString(MembershipCreateStatus createStatus)
        {
            // See http://go.microsoft.com/fwlink/?LinkID=177550 for
            // a full list of status codes.
            switch (createStatus)
            {
                case MembershipCreateStatus.DuplicateUserName:
                    return "Username already exists. Please enter a different user name.";

                case MembershipCreateStatus.DuplicateEmail:
                    return "A username for that e-mail address already exists. Please enter a different e-mail address.";

                case MembershipCreateStatus.InvalidPassword:
                    return "The password provided is invalid. Please enter a valid password value.";

                case MembershipCreateStatus.InvalidEmail:
                    return "The e-mail address provided is invalid. Please check the value and try again.";

                case MembershipCreateStatus.InvalidAnswer:
                    return "The password retrieval answer provided is invalid. Please check the value and try again.";

                case MembershipCreateStatus.InvalidQuestion:
                    return "The password retrieval question provided is invalid. Please check the value and try again.";

                case MembershipCreateStatus.InvalidUserName:
                    return "The user name provided is invalid. Please check the value and try again.";

                case MembershipCreateStatus.ProviderError:
                    return "The authentication provider returned an error. Please verify your entry and try again. If the problem persists, please contact your system administrator.";

                case MembershipCreateStatus.UserRejected:
                    return "The user creation request has been canceled. Please verify your entry and try again. If the problem persists, please contact your system administrator.";

                default:
                    return "An unknown error occurred. Please verify your entry and try again. If the problem persists, please contact your system administrator.";
            }
        }
    }

    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property, AllowMultiple = false, Inherited = true)]
    public sealed class ValidatePasswordLengthAttribute : ValidationAttribute, IClientValidatable
    {
        private const string _defaultErrorMessage = "'{0}' must be at least {1} characters long.";
        private readonly int _minCharacters = Membership.Provider.MinRequiredPasswordLength;

        public ValidatePasswordLengthAttribute()
            : base(_defaultErrorMessage)
        {
        }

        public override string FormatErrorMessage(string name)
        {
            return String.Format(CultureInfo.CurrentCulture, ErrorMessageString,
                name, _minCharacters);
        }

        public override bool IsValid(object value)
        {
            string valueAsString = value as string;
            return (valueAsString != null && valueAsString.Length >= _minCharacters);
        }

        public IEnumerable<ModelClientValidationRule> GetClientValidationRules(ModelMetadata metadata, ControllerContext context)
        {
            return new[]{
                new ModelClientValidationStringLengthRule(FormatErrorMessage(metadata.GetDisplayName()), _minCharacters, int.MaxValue)
            };
        }
    }
    #endregion

}
