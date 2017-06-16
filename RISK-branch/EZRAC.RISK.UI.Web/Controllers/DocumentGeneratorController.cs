using EZRAC.Risk.UI.Web.DocumentGenerator;
using EZRAC.Risk.UI.Web.Helper;
using EZRAC.Risk.UI.Web.ViewModels.Claims;
using EZRAC.Risk.UI.Web.ViewModels.DocumentGenerator;
using EZRAC.RISK.Services.Contracts;
using EZRAC.RISK.Util.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace EZRAC.Risk.UI.Web.Controllers
{
    [CRMSAuthroize(ClaimType = ClaimsConstant.DocumentGenerator)]
    public class DocumentGeneratorController : Controller
    {

        private IDocumentGeneratorService _documentService;
        private IBillingService _billingService;
        private IPaymentService _paymentService;


        public DocumentGeneratorController(IDocumentGeneratorService documentService, IBillingService billingService, IPaymentService paymentService)
        {
            _documentService = documentService;
            _billingService = billingService;
            _paymentService = paymentService;
        }

        public async Task<PartialViewResult> Index()
        {
            var listDto = await _documentService.GetAllDocumentTypesAsync();
            var viewmodel = DocumentGeneratorHelper.GetDocumentCategoriesViewModel(listDto);
            return PartialView("_Index",viewmodel);
        }

        public async Task<ActionResult> GetDocumentType(long claimId,int documentType,string title)
        {
            var viewModel = await GetDocumentGeneratorViewModel(claimId, (DocumentTypes)documentType, title);
            return PartialView("_DocumentGeneratorForm",viewModel);
        }

        [HttpGet]
        public async Task<ActionResult> GenerateDocument(DocumentGeneratorViewModel viewModel)
        {
            byte[] pdfBytes = null;
            //PdfResult pdfResult = null;
            IDocumentGenerator documentGenerator = DocumentFactory.GetInstance(viewModel.DocumentTypeId);

            if (documentGenerator != null)
            {
                pdfBytes = await documentGenerator.GetBytesAsync(viewModel);

                UserActionLogHelper.AddUserActionLog(GetAcionDescription(viewModel.DocumentTypeId), viewModel.ClaimId);

                //pdfResult = await documentGenerator.GetPdfResultAsync(viewModel);
            }

            return File(pdfBytes, System.Net.Mime.MediaTypeNames.Application.Pdf);
            //return pdfResult;
        }

        private static string GetAcionDescription(DocumentTypes documentTypes)
        {
            string actionDescription = string.Empty;

            if (documentTypes!=null)
            {
                actionDescription = String.Format("Generated {0}", documentTypes.ToString().Replace("_", " "));
            }

            return actionDescription;
        }

        private async Task<DocumentGeneratorViewModel> GetDocumentGeneratorViewModel(long claimId, DocumentTypes documentType, string title)
        {
            DocumentGeneratorViewModel viewModel = new DocumentGeneratorViewModel();

            var billingsDto = await _billingService.GetBillingsByClaimIdAsync(claimId);
            BillingViewModel model = ClaimHelper.GetBillingsViewModel(billingsDto, null, false);

            var driversDto = await _documentService.GetDriversByClaimAsync(claimId);

            if (documentType == DocumentTypes.Demand_Letter_1_3rd_Party || documentType == DocumentTypes.Demand_Letter_1_3rd_Party_Insurance)
            {
                driversDto = driversDto.Where(x => x.DriverTypeId == (int)ClaimsConstant.DriverTypes.ThirdParty).ToList();
            }

            var driversList =  LookUpHelpers.GetSelectListItems(driversDto);


            viewModel.Title = title;
            viewModel.Billings = model;
            viewModel.Drivers = driversList;
            viewModel.DocumentTypeId = documentType;
            viewModel.ClaimId = claimId;
            viewModel.TotalBill = model.Billings != null && model.Billings.Any() ? model.Billings.Sum(x => x.Amount * (1 - (x.Discount / 100))) : default(double);
            var payment = await _paymentService.GetTotalPayments(claimId);
            viewModel.TotalPayment = payment != null ? payment.Value : default(double);
            Nullable<double> due = (viewModel.TotalBill.HasValue ? viewModel.TotalBill.Value : default(double)) - (viewModel.TotalPayment.HasValue ? viewModel.TotalPayment.Value : default(double));
            viewModel.TotalDue = due > 0 ? Math.Round(due.HasValue ? due.Value : default(double), 2) : default(double);
            //ViewBag.Drivers = driversList;
            return viewModel;
        }

	}
}