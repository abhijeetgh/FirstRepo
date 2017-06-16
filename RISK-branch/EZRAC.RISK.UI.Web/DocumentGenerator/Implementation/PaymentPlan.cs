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
    public class PaymentPlan : IDocumentGenerator
    {
        IDocumentGeneratorService _documentGeneratorService = null;
        

        public PaymentPlan(IDocumentGeneratorService documentGeneratorService)
        {
            _documentGeneratorService = documentGeneratorService;
        }

        public async Task<byte[]> GetBytesAsync(DocumentGeneratorViewModel request)
        {

            byte[] pdfBytes = null;

            if (request != null)
            {

                PaymentPlanViewModel viewModel = new PaymentPlanViewModel();


                DocumentHeaderDto documentHeaderDto = await _documentGeneratorService.GetDocumentHeaderInfoAsync(request.ClaimId, request.SelectedDriverId);

                viewModel.HeaderViewModel = DocumentGeneratorHelper.GetDocumentTemplateViewModel(documentHeaderDto);

                viewModel.HeaderViewModel.TotalDue = await _documentGeneratorService.GetTotalDueAsync(request.SelectedBillings, request.ClaimId);

                viewModel.Payments = GetDuePaymentsPlan(viewModel.HeaderViewModel.TotalDue, request.NumberOfMonths.HasValue ? request.NumberOfMonths.Value : 1);

                viewModel.HeaderViewModel.DateOfLoss = documentHeaderDto.DateOfLoss;

                string template = GetTemplate((DocumentTypes)request.DocumentTypeId, viewModel);

                pdfBytes = PDFHelper.PdfSharpConvertBytes(template);

            }

            return pdfBytes;

        }


        private static string GetTemplate(DocumentTypes docType, PaymentPlanViewModel model)
        {
            string template = string.Empty;
            if (docType == DocumentTypes.Payment_Plan)
            {

                template = RazorHelper.ParseTemplate(DocumemtTemplateUrl.Payment_Plan, model);
            }
            return template;
        }


        private static List<PaymentsDuePlan> GetDuePaymentsPlan(double amount, int numberofMonths)
        {
            var listPayments = new List<PaymentsDuePlan>();
            PaymentsDuePlan payment = null;
            double plannedAmount = amount / numberofMonths;
            for (int addMonths = 1; addMonths <= numberofMonths; addMonths++)
            {
                payment = new PaymentsDuePlan();
                payment.DateOfPayment = GetPaymentDate(addMonths);
                payment.Amount = Math.Round(plannedAmount, 2);
                listPayments.Add(payment);
            }
            return listPayments;
        }

        private static string GetPaymentDate(int addMonth)
        {
            var date = ConfigSettingsReader.GetAppSettingValue(Constants.AppSettings.PaymentPlanDate.ToString()) +" "+DateTime.Now.AddMonths(addMonth).ToString(Constants.CrmsDateFormates.Y);
            return date;
        }
    }
}