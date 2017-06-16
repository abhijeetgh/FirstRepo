using EZRAC.Risk.UI.Web.Helper;
using EZRAC.RISK.Services.Contracts;
using EZRAC.RISK.Services.Contracts.Dtos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.Mvc;

namespace EZRAC.Risk.UI.Web.Attributes
{
    public sealed class UserActionLogAttribute : ActionFilterAttribute
    {
        public string UserAction = string.Empty;

        private static readonly Regex UrlId = new Regex("[0-9]+$");

        private static string ExtractID(string url)
        {
            var match = UrlId.Match(url);
            return match.Success
                ? match.Captures[0].Value
                : string.Empty;
        }

        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            if (filterContext.HttpContext.User.Identity.IsAuthenticated)
            {
                IUserService userService = UnityResolver.ResolveService<IUserService>();

                long claimId = 0;

                if (filterContext.HttpContext.Session["ClaimId"] != null)
                {
                    long.TryParse(filterContext.HttpContext.Session["ClaimId"].ToString(), out claimId);
                }
                else
                {
                    if (filterContext.HttpContext.Request.Path.Contains("/Claims/ViewClaim"))
                    {
                        long.TryParse(ExtractID(filterContext.HttpContext.Request.Path), out claimId);
                    }
                }
                UserActionLogDto userActionLog = new UserActionLogDto
                {
                    ClaimId = claimId,
                    Date = DateTime.Now,
                    UserId = SecurityHelper.GetUserIdFromContext(),
                    UserAction = UserAction
                };
                userService.AddUserActionLog(userActionLog);
            }
        }

        
    }
}