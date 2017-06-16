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
    public class ThirdPartyInsuranceDemandLetter : IDocumentGenerator
    {
         IDocumentGeneratorService _documentGeneratorService = null;
         IClaimService _claimService;
         IPaymentService _paymentsService;

         public ThirdPartyInsuranceDemandLetter(IDocumentGeneratorService documentGeneratorService, IClaimService claimService,IPaymentService paymentService)
         {
             _documentGeneratorService = documentGeneratorService;
             _claimService = claimService;
             _paymentsService = paymentService;
         }

        public async Task<byte[]> GetBytesAsync(DocumentGeneratorViewModel request)
        {
            byte[] pdfBytes = null;

            if (request != null)
            {

                DocumentHeaderDto documentHeaderDto = await _documentGeneratorService.GetDocumentHeaderInfoAsync(request.ClaimId, request.SelectedDriverId);

                IEnumerable<RiskBillingDto> billingsDtoList = await _documentGeneratorService.GetSelectedBillingsByIdsAsync(request.SelectedBillings);

                ContractDto contractDto = await _claimService.GetContractByClaimIdAsync(request.ClaimId);

                var paymentsDto = await _paymentsService.GetAllPaymentsByClaimId(request.ClaimId);
               
                InsuranceDto insuranceDto = await _documentGeneratorService.GetInsuranceInfoByDriverId(request.SelectedDriverId);

                DocumentTemplateViewModel model = await GetTemplateViewModel(request, documentHeaderDto, billingsDtoList, contractDto, insuranceDto, paymentsDto);

                string template = GetTemplate(request.DocumentTypeId, model);

               
                pdfBytes = PDFHelper.PdfSharpConvertBytes(template);

            }
            return pdfBytes;
        }

        private async Task<DocumentTemplateViewModel> GetTemplateViewModel(DocumentGeneratorViewModel request, DocumentHeaderDto documentHeaderDto, 
                                               IEnumerable<RiskBillingDto> billingsDto, ContractDto contractDto,InsuranceDto insuranceDto,PaymentDto paymentsDto)
        {
            DocumentTemplateViewModel model = DocumentGeneratorHelper.GetDocumentTemplateViewModel(documentHeaderDto);

            model.DriverViewModel.InsuranceViewModel = DocumentGeneratorHelper.MapInsuranceViewModel(insuranceDto);

            model.Contract = DocumentGeneratorHelper.MapContractViewModel(contractDto);

            model.Billings = DocumentGeneratorHelper.GetBillingViewModel(billingsDto);

            model.Payments = DocumentGeneratorHelper.GetPaymentsViewModel(paymentsDto);

            model.DateOfLoss = documentHeaderDto.DateOfLoss;

            model.TotalDue = await _documentGeneratorService.GetTotalDueAsync(request.SelectedBillings, request.ClaimId);

            return model;
        }

        private static string GetTemplate(DocumentTypes docType, DocumentTemplateViewModel model)
        {
            string template = string.Empty;
            if (docType == DocumentTypes.Demand_Letter_1_3rd_Party_Insurance)
            {

                template = RazorHelper.ParseTemplate(DocumemtTemplateUrl.Demand_Letter_1_3rd_Party_Insurance, model);
            }
            else if (docType == DocumentTypes.Demand_Letter_1_Renter_Insurance)
            {

                template = RazorHelper.ParseTemplate(DocumemtTemplateUrl.Demand_Letter_1_Renter_Insurance, model);
            }
           

            return template;
        }

       
    }
}