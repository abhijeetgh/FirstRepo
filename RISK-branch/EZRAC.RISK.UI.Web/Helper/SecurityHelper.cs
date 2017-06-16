using EZRAC.RISK.Services.Contracts;
using EZRAC.RISK.Services.Contracts.Dtos;
using EZRAC.RISK.Util.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Web;

namespace EZRAC.Risk.UI.Web.Helper
{
    public static class SecurityHelper
    {
        public static bool IsAuthorized(string claimType)
        {
            var hasAccess = false;
            HttpContext context = System.Web.HttpContext.Current;

            if (context.User.Identity.IsAuthenticated)
            {
                var identity = (System.Security.Claims.ClaimsIdentity)context.User.Identity;

                hasAccess = identity.Claims.Any(x => x.Value == claimType);

                
            }
           // hasAccess = true;
            return hasAccess;
        }

        public static bool IsAdminAccess()
        {
            var hasAccess = false;

            var roles = ConfigSettingsReader.GetAppSettingValue(ClaimsConstant.AdminAccessRoles); //Take it from Config file

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

            return hasAccess;
        }

        public static bool IsInRole(string role) {

            var hasAccess = false;

            HttpContext context = System.Web.HttpContext.Current;

            if (context.User.Identity.IsAuthenticated)
            {
                
                    if (context.User.IsInRole(role))
                    {
                        hasAccess = true;
                    }
                
            }

            return hasAccess;
        }

        public static long GetUserIdFromContext()
        {
            long userId =0;
            HttpContext context = System.Web.HttpContext.Current;

            if (context.User.Identity.IsAuthenticated)
            {
                var identity = (System.Security.Claims.ClaimsIdentity)context.User.Identity;
                userId = Convert.ToInt64((identity.Claims.Where(c => c.Type == ClaimTypes.Sid).Select(c => c.Value)).FirstOrDefault());
            }
            return userId;
        }

        public static string GetUserNameFromContext()
        {
            var userName = string.Empty;
            HttpContext context = System.Web.HttpContext.Current;

            if (context.User.Identity.IsAuthenticated)
            {
                var identity = (System.Security.Claims.ClaimsIdentity)context.User.Identity;
                userName = Convert.ToString((identity.Claims.Where(c => c.Type == ClaimTypes.Name).Select(c => c.Value)).FirstOrDefault());
            }
            return userName;
        }

        public static bool IsCreatorOrAuthorized(string claimType,long claimId)
        {
            var hasAccess = false;
            IClaimService _claimService =UnityResolver.ResolveService<IClaimService>();
            HttpContext context = System.Web.HttpContext.Current;

            if (context.User.Identity.IsAuthenticated)
            {
                if (IsInRole(ClaimsConstant.Roles.LocationManager))
                {
                    hasAccess = _claimService.IsClaimCreator(GetUserIdFromContext(), claimId);
                }
                
                if (!hasAccess)
                {
                    var identity = (System.Security.Claims.ClaimsIdentity)context.User.Identity;

                    hasAccess = identity.Claims.Any(x => x.Value == claimType);
                }
            }
            // hasAccess = true;
            return hasAccess;
        }

        public static bool IsAuthorizedForReports()
        {
            bool isSuccess = true;
            IRiskReport _riskReport = UnityResolver.ResolveService<IRiskReport>();

            isSuccess = _riskReport.IsAuthorizedForReports(SecurityHelper.GetUserIdFromContext());

            return isSuccess;
        }

        public static bool IsAuthorizedForTracking()
        {
            bool isSuccess = false;

            HttpContext context = System.Web.HttpContext.Current;

            if (context.User.Identity.IsAuthenticated)
            {
                var identity = (System.Security.Claims.ClaimsIdentity)context.User.Identity;

                ITrackingService _trackingService = UnityResolver.ResolveService<ITrackingService>();

                IEnumerable<TrackingTypeDto> trackingCategories =  _trackingService.GetTrackingCategoriesAsync();

                isSuccess = identity.Claims.Any(x => trackingCategories.Any(y => y.Key == x.Value));

            }

            return isSuccess;
        }



    }
}