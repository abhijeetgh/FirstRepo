using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using Microsoft.Owin.Security;
using RateShopper.Domain.Entities;
using RateShopper.Services.Data;

namespace RateShopper.Controllers
{
    [Authorize]
    public class AccountController : Controller
    {
        private IUserService _userService;

        public AccountController(IUserService userservice)
        {
            _userService = userservice;
        }


        //
        // GET: /Account/Login
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1054:UriParametersShouldNotBeStrings", Justification="This parameter holds query string value not whole url. If URI is used then maximum of its properties threw an exception. So string is used here", MessageId = "0#"), AllowAnonymous]
        public ActionResult Login(string returnUrl)
        {
            ViewBag.ReturnUrl = returnUrl;            
            return View();
        }

        //
        // POST: /Account/Login
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public ActionResult Login(LoginViewModel model, string redirectTo)
        {
            if (model != null && ModelState.IsValid)
            {                
                //Validate user
                User user = _userService.ValidateUser(model.UserName, model.Password);
                if (user != null)
                {
                    var claimId = _userService.SignInUser(user);
                    AuthenticationManager.SignIn(claimId);

                    if (Url.IsLocalUrl(redirectTo))
                    {
                        return Redirect(redirectTo);
                    }
                    else
                    {
                        return RedirectToAction("Index", "Search");
                    }

                }

                ModelState.AddModelError("", Resource_Files.CustomMessages.Login_ErrorMessageInvalidUser);
            }

            return View(model);
        }

        private IAuthenticationManager AuthenticationManager
        {
            get
            {
                return HttpContext.GetOwinContext().Authentication;
            }
        }

        public ActionResult RemoveAuthoriseUserData()
        {
            AuthenticationManager.SignOut();
            return RedirectToAction("Login", "Account");
        }

        // POST: /Account/LogOff
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult LogOff()
        {
            return RemoveAuthoriseUserData();
        }


    }

    
}