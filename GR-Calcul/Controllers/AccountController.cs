using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Security.Principal;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using System.Web.Security;
using GR_Calcul.Models;
using GR_Calcul.Misc;

namespace GR_Calcul.Controllers
{
    public class AccountController : BaseController
    {

        private PersonModel personModel = new PersonModel();
        private LostPasswordChangeModel lostPwdChangeModel = new LostPasswordChangeModel();

        public IFormsAuthenticationService FormsService { get; set; }
        public IMembershipService MembershipService { get; set; }

        protected override void Initialize(RequestContext requestContext)
        {
            if (FormsService == null) { FormsService = new FormsAuthenticationService(); }
            if (MembershipService == null) { MembershipService = new AccountMembershipService(); }

            base.Initialize(requestContext);
        }

        // **************************************
        // URL: /Account/LogOn
        // **************************************

        public ActionResult LogOn()
        {
            if (SessionManager.IsLogged(HttpContext))
            {
                return new HomeController().Index();
            }else
                return View();
        }

        [HttpPost]
        public ActionResult LogOn(LogOnModel model, string returnUrl)
        {
            if (ModelState.IsValid)
            {
                if (MembershipService.ValidateUser(model.UserName, model.Password))
                {
                    FormsService.SignIn(model.UserName, model.RememberMe);
                    if (Url.IsLocalUrl(returnUrl))
                    {
                        return Redirect(returnUrl);
                    }
                    else
                    {
                        return RedirectToAction("Index", "Home");
                    }
                }
                else
                {
                    ModelState.AddModelError("", "Le nom d'utilisateur et/ou le mot de passe est incorrect.");
                }
            }

            // If we got this far, something failed, redisplay form
            return View(model);
        }
        public ActionResult AccessDenied()
        {
            return View();
        }

        // **************************************
        // URL: /Account/LogOff
        // **************************************

        public ActionResult LogOff()
        {
            FormsService.SignOut();

            return RedirectToAction("Index", "Home");
        }

        //
        // GET: /AccountController/LostPasswordChange/hgjfgdsajhfgjhsadfg
        //public ActionResult LostPasswordChange(NonNullable<String> token)
        public ActionResult LostPasswordChange(string token)
        {
            if (token == null)
            {
                throw new ArgumentNullException();
            }
            if (lostPwdChangeModel.IsTokenValid(token))
            {
                ViewBag.token = token;
                ViewBag.PasswordLength = MembershipService.MinPasswordLength;
                return View();
            }
            else
            {
                throw new Exception("Invalid token. The token may have expired.");
            }
        }

        [HttpPost]
        public ActionResult LostPasswordChange(LostPasswordChangeModel model, string token)
        {
            if (ModelState.IsValid)
            {
                if (MembershipService.ChangePassword(token, model.NewPassword))
                {
                    return RedirectToAction("ChangePasswordSuccess");
                }
                else
                {
                    ModelState.AddModelError("", "Le nouveau mot de passe est invalide.");
                }
            }

            // If we got this far, something failed, redisplay form
            ViewBag.PasswordLength = MembershipService.MinPasswordLength;
            return View(model);

        }


        public ActionResult LostPassword()
        {
            if (SessionManager.IsLogged(HttpContext))
            {
                return RedirectToAction("Index", "Home");
            }
            else
            {
                return View();
            }
        }

        [HttpPost]
        public ActionResult LostPassword(LostPasswordModel model)
        {
            if (ModelState.IsValid)
            {
                if (Membership.GetUserNameByEmail(model.Email) != null)
                {
                    return View("LostPasswordSuccess", model);
                }
                else
                {
                    ModelState.AddModelError("", "L'adresse email saisie n'a pas été trouvée dans notre système.");
                }
            }

            // If we got this far, something failed, redisplay form
            return View(model);
        }

        // **************************************
        // URL: /Account/ChangePassword
        // **************************************

        [DuffAuthorize(PersonType.Responsible, PersonType.ResourceManager, PersonType.User)]
        public ActionResult ChangePassword()
        {
            ViewBag.PasswordLength = MembershipService.MinPasswordLength;
            return View();
        }

        [HttpPost]
        [DuffAuthorize(PersonType.Responsible, PersonType.ResourceManager, PersonType.User)]
        public ActionResult ChangePassword(ChangePasswordModel model)
        {
            if (ModelState.IsValid)
            {
                if (MembershipService.ChangePassword(User.Identity.Name, model.OldPassword, model.NewPassword))
                {
                    return RedirectToAction("ChangePasswordSuccess");
                }
                else
                {
                    ModelState.AddModelError("", "Le mot de passe courant est incorrect ou le nouveau mot de passe est invalide.");
                }
            }

            // If we got this far, something failed, redisplay form
            ViewBag.PasswordLength = MembershipService.MinPasswordLength;
            return View(model);
        }

        // **************************************
        // URL: /Account/ChangePasswordSuccess
        // **************************************
        //[DuffAuthorize(PersonType.Responsible, PersonType.ResourceManager, PersonType.User)]
        //allow everyone so that we can use the same view for "lost password change" in which case the user is not logged
        public ActionResult ChangePasswordSuccess()
        {
            return View();
        }

    }
}
