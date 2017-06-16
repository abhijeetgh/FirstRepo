using EZRAC.Core.FileGenerator;
using EZRAC.Core.Util;
using EZRAC.Risk.UI.Web.Helper;
using EZRAC.Risk.UI.Web.ViewModels.DocumentGenerator;
using EZRAC.RISK.Services.Contracts;
using EZRAC.RISK.Services.Contracts.Dtos;
using EZRAC.RISK.Util.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;

namespace EZRAC.Risk.UI.Web.DocumentGenerator.Implementation
{
    public class PaymentBalanceDue : IDocumentGenerator
    {
        IDocumentGeneratorService _documentGeneratorService = null;
        IPaymentService _paymentService = null;

        public PaymentBalanceDue(IDocumentGeneratorService documentGeneratorService, IPaymentService paymentService)
        {
            _documentGeneratorService = documentGeneratorService;
            _paymentService = paymentService;
        }

        public async Task<byte[]> GetBytesAsync(DocumentGeneratorViewModel request)
        {

            byte[] pdfBytes = null;

            if (request != null)
            {

                PaymentDueViewModel viewModel = new PaymentDueViewModel();

                DocumentHeaderDto documentHeaderDto = await _documentGeneratorService.GetDocumentHeaderInfoAsync(request.ClaimId, request.SelectedDriverId);

                viewModel.HeaderViewModel = DocumentGeneratorHelper.GetDocumentTemplateViewModel(documentHeaderDto); ;

                var paymentsDto = await _paymentService.GetAllPaymentsByClaimId(request.ClaimId);

                viewModel.Payments = PaymentHelper.GetPaymentsInfo(paymentsDto);

                viewModel.HeaderViewModel.TotalDue = await _documentGeneratorService.GetTotalDueAsync(request.SelectedBillings, request.ClaimId);

                viewModel.OriginalDue = await _documentGeneratorService.GetOriginalBalanceAsync(request.SelectedBillings, request.ClaimId);

                viewModel.HeaderViewModel.DateOfLoss = documentHeaderDto.DateOfLoss;

                string template = GetTemplate((DocumentTypes)request.DocumentTypeId, viewModel);

                pdfBytes = PDFHelper.PdfSharpConvertBytes(template);

            }


            return pdfBytes;

        }


        private static string GetTemplate(DocumentTypes docType, PaymentDueViewModel model)
        {
            string template = string.Empty;
            if (docType == DocumentTypes.Payment_Balance_due)
            {

                template = RazorHelper.ParseTemplate(DocumemtTemplateUrl.Payment_Balance_due, model);
            }
            else if (docType == DocumentTypes.Payment_Claim_Closed)
            {
                template = RazorHelper.ParseTemplate(DocumemtTemplateUrl.Payment_Claim_Closed, model);
            }
            return template;
        }
    }
}