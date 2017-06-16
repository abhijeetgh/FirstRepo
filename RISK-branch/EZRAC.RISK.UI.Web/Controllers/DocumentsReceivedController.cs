using EZRAC.Risk.UI.Web.Helper;
using EZRAC.Risk.UI.Web.ViewModels.DocumentsReceived;
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
    public class DocumentsReceivedController : Controller
    {
        private IDocumentsReceivedService _documentsReceivedService = null;

        public DocumentsReceivedController(IDocumentsReceivedService documentsReceivedService)
        {
            _documentsReceivedService = documentsReceivedService;
            
        }
        //
        // GET: /DocumentsReceived/
        public async Task<ActionResult> Index(long id)
        {

            DocumentsReceivedDto documentsReceivedDto = await _documentsReceivedService.GetDocumentsReceivedByClaimIdAsync(id);

            DocumentsReceivedViewModel model = DocumentsReceivedHelper.GetDocumentsReceivedViewModel(documentsReceivedDto);

            model.ClaimId = id;

            return PartialView(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [CRMSAuthroizeAttribute(ClaimType = ClaimsConstant.EditDocumentsReceived)]
        public async Task<ActionResult> EditDocumentsReceived(DocumentsReceivedViewModel documentsReceivedViewModel) 
        {
            bool isSuccess = false;
            if (documentsReceivedViewModel !=null)
            {
                DocumentsReceivedDto documentsReceivedDto = DocumentsReceivedHelper.GetDocumentsReceivedDto(documentsReceivedViewModel);

                isSuccess = await _documentsReceivedService.UpdateDocumentsReceivedAsync(documentsReceivedDto);

            }
            return Json(new { IsSuccess = isSuccess }, JsonRequestBehavior.AllowGet);
        }
	}
}