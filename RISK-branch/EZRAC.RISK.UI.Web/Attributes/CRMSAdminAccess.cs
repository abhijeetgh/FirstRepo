using EZRAC.Risk.UI.Web.Helper;
using EZRAC.RISK.Util.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;

namespace EZRAC.Risk.UI.Web.Attributes
{
    public sealed class CRMSAdminAccess : AuthorizeAttribute
    {
        public string AdminRoles = string.Empty;

        public override void OnAuthorization(AuthorizationContext filterContext)
        {
            base.OnAuthorization(filterContext);

            bool hasAccess = false;

            if (filterContext.HttpContext.User.Identity.IsAuthenticated)
            {

                var roles = String.IsNullOrEmpty(AdminRoles) ? ConfigSettingsReader.GetAppSettingValue(ClaimsConstant.AdminAccessRoles) : AdminRoles; //Take it from Config file

                HttpContext context = System.Web.HttpContext.Current;

                if (context.User.Identity.IsAuthenticated)
                {
                    foreach (string role in roles.Split(','))
                    {
                        if (context.User.IsInRole(role))
                        {
                            hasAccess = true;
                        }
                    }
                }
                //hasAccess = true;
                if (!hasAccess)
                    filterContext.Result = new HttpForbiddenResult();
            }
            else
            {
                filterContext.Result = new HttpUnauthorizedResult();
            }
        }
    }

}
