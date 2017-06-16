using EZRAC.Risk.UI.Web.Models;
using EZRAC.RISK.Services.Contracts;
using EZRAC.RISK.Services.Contracts.Dtos;
using Microsoft.AspNet.Identity;
using Microsoft.Owin.Security;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace EZRAC.Risk.UI.Web.Controllers
{
    [Authorize]
    public class AccountController : Controller
    {
        private IUserService _userService;

        public AccountController(IUserService userService)
        {
            _userService = userService;
        }       

        //
        // GET: /Account/Login
        [AllowAnonymous]
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
        public async Task<ActionResult> Login(LoginViewModel model, string returnUrl)
        {
            if (ModelState.IsValid)
            {
                //var result = await WebApiService.Instance.AuthenticateAsync<SignInResult>("cybage", "cybage");

                //var ticket = OAuthBearerOptions.AccessTokenFormat.Unprotect(AccessToken);
                //var id = new ClaimsIdentity(ticket.Identity.Claims, DefaultAuthenticationTypes.ApplicationCookie);
         
                var user = await _userService.ValidateUserAsync(model.UserName, model.Password);
                if (user != null && user.ErroMsg=="IsValid")
                {
                    await SignInAsync(user, model.RememberMe);
                    return RedirectToLocal(returnUrl);
                }
                else
                {
                    if (user.ErroMsg == "IsInValid")
                    {
                        ModelState.AddModelError("", "Invalid username or password.");
                    }
                    else
                    {
                        ModelState.AddModelError("", "User is inactive.");
                    }
                }
            }

            // If we got this far, something failed, redisplay form
            return View(model);
        }            
       
        //
        // POST: /Account/LogOff
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult LogOff()
        {
            AuthenticationManager.SignOut();
            return RedirectToAction("Login", "Account");
        }      

        protected override void Dispose(bool disposing)
        {
            if (disposing && _userService != null)
            {
                _userService = null;
            }
            base.Dispose(disposing);
        }

        #region Helpers
        // Used for XSRF protection when adding external logins
        private const string XsrfKey = "XsrfId";

        private IAuthenticationManager AuthenticationManager
        {
            get
            {
                return HttpContext.GetOwinContext().Authentication;
            }
        }

        private async Task SignInAsync(UserDto user, bool isPersistent)
        {
            AuthenticationManager.SignOut(DefaultAuthenticationTypes.ExternalCookie);
            //var identity = await UserManager.CreateIdentityAsync(user, DefaultAuthenticationTypes.ApplicationCookie);
            var identity = await _userService.GetClaimsIdentityAsync(user);
            AuthenticationManager.SignIn(identity);
            
        }

        private void AddErrors(IdentityResult result)
        {
            foreach (var error in result.Errors)
            {
                ModelState.AddModelError("", error);
            }
        }
      

        public enum ManageMessageId
        {
            ChangePasswordSuccess,
            SetPasswordSuccess,
            RemoveLoginSuccess,
            Error
        }

        private ActionResult RedirectToLocal(string returnUrl)
        {
            if (Url.IsLocalUrl(returnUrl) && !returnUrl.Contains("Account/LogOff"))
            {
                return Redirect(returnUrl);
            }
            else
            {
                return RedirectToAction("GetClaimList", "Claims");
            }
        }

        private class ChallengeResult : HttpUnauthorizedResult
        {
            //public ChallengeResult(string provider, string redirectUri) : this(provider, redirectUri, null)
            //{
            //}

            //public ChallengeResult(string provider, string redirectUri, string userId)
            //{
            //    LoginProvider = provider;
            //    RedirectUri = redirectUri;
            //    UserId = userId;
            //}

            //public string LoginProvider { get; set; }
            //public string RedirectUri { get; set; }
            //public string UserId { get; set; }

            //public override void ExecuteResult(ControllerContext context)
            //{
            //    var properties = new AuthenticationProperties() { RedirectUri = RedirectUri };
            //    if (UserId != null)
            //    {
            //        properties.Dictionary[XsrfKey] = UserId;
            //    }
            //    context.HttpContext.GetOwinContext().Authentication.Challenge(properties, LoginProvider);
            //}
        }
        #endregion
    }
}