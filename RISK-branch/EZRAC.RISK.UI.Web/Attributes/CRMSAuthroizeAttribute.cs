using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using System.Web.Mvc.Filters;

namespace EZRAC.Risk.UI.Web
{
    public sealed class CRMSAuthroizeAttribute : AuthorizeAttribute 
    {  
        public string ClaimType = string.Empty;

        public override void OnAuthorization(AuthorizationContext filterContext)
        {
            base.OnAuthorization(filterContext);

            bool hasAccess = false;

            if (filterContext.HttpContext.User.Identity.IsAuthenticated)
            {
                var identity = (System.Security.Claims.ClaimsIdentity)filterContext.HttpContext.User.Identity;
                hasAccess = identity.Claims.Any(x => x.Value == ClaimType);

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
         

    public class HttpForbiddenResult : HttpStatusCodeResult
    {
        public HttpForbiddenResult()
            : this(null)
        {
        }

        public HttpForbiddenResult(string statusDescription)
            : base(HttpStatusCode.Forbidden, statusDescription)
        {
        }
    } 
}
