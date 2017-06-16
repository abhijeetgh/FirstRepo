using EZRAC.RISK.Services.Contracts;
using EZRAC.RISK.Services.Contracts.Dtos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace EZRAC.Risk.UI.Web.Helper
{
    public class UserActionLogHelper
    {
        private static IUserService _userService = UnityResolver.ResolveService<IUserService>();

        public static void AddUserActionLog(string userAction, long claimId=0)
        {
            UserActionLogDto userActionLog = new UserActionLogDto
               {
                   ClaimId = claimId,
                   Date = DateTime.Now,
                   UserId = SecurityHelper.GetUserIdFromContext(),
                   UserAction = userAction
               };
            _userService.AddUserActionLog(userActionLog);
        }
    }
}