using EZRAC.Risk.UI.Web.ViewModels.Claims;
using EZRAC.RISK.Services.Contracts;
using EZRAC.RISK.Services.Contracts.Dtos;
using EZRAC.RISK.Util.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace EZRAC.Risk.UI.Web.Controllers
{
    public class ClaimEventsController : Controller
    {

        private IUserService _userService;

        public ClaimEventsController(IUserService userService)
        {
            _userService = userService;
        }
        // GET: /ClaimEvents/
        public async Task<ActionResult> Index(long ClaimId, string CompAbbr)
        {
            ViewBag.ClaimId = ClaimId;
            ViewBag.CompanyAbbreviation = CompAbbr;
            return PartialView();
        }

        //public async Task<ActionResult> GetClaims(long ClaimId)
        //{
        //    var userActionLogDtos = await _userService.GetUserActionsLogByClaimIdAsync(ClaimId);

        //    var userActionUserActionModelList = MapUserActionLogDto(userActionLogDtos);

        //    return PartialView(userActionUserActionModelList);
        //}

        [HttpPost]
        [CRMSAuthroize(ClaimType=ClaimsConstant.ClaimEvent)]
        public async Task<ActionResult> GetClaims(UserActionLogViewModel userViewModel)
        {
            long claimId= 0;

            long.TryParse(userViewModel.ClaimId, out claimId);

            var userActionLogDtos = await _userService.GetUserActionsLogByClaimIdAsync(claimId, userViewModel.FromDate, userViewModel.ToDate);

            var userActionUserActionModelList = MapUserActionLogDto(userActionLogDtos);

            return PartialView("_GetClaims", userActionUserActionModelList);
        }
        private static List<UserActionLogViewModel> MapUserActionLogDto(IEnumerable<UserActionLogDto> userActionLogDtos )
        {
            return userActionLogDtos.Select(x =>
                new UserActionLogViewModel 
                {
                    ClaimId=x.ClaimId.ToString(),
                    Name=x.Name,
                    UserName= x.UserName,
                    UserAction=x.UserAction,
                    Date=x.Date.ToString(CommonConstant.DateFormat)
                }).OrderByDescending(y=>y.Date).ToList();
        }
	}
}