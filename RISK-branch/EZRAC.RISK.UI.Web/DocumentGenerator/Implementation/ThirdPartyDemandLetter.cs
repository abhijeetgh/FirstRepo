using EZRAC.Core.FileGenerator;
using EZRAC.Core.Util;
using EZRAC.Risk.UI.Web.Helper;
using EZRAC.Risk.UI.Web.ViewModels.Claims;
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
    public class ThirdPartyDemandLetter: IDocumentGenerator
    {

         IDocumentGeneratorService _documentGeneratorService = null;
         IClaimService _claimService;

         public ThirdPartyDemandLetter(IDocumentGeneratorService documentGeneratorService,IClaimService claimService)
         {
             _documentGeneratorService = documentGeneratorService;
             _claimService = claimService;
         }

        public async Task<byte[]> GetBytesAsync(DocumentGeneratorViewModel request)
        {
            byte[] pdfBytes = null;

            if (request != null)
            {

                DocumentHeaderDto documentHeaderDto = await _documentGeneratorService.GetDocumentHeaderInfoAsync(request.ClaimId, request.SelectedDriverId);

                IEnumerable<RiskBillingDto> billingsDtoList = await _documentGeneratorService.GetSelectedBillingsByIdsAsync(request.SelectedBillings);

                ContractDto contractDto = await _claimService.GetContractByClaimIdAsync(request.ClaimId);

                DocumentTemplateViewModel model = await GetTemplateViewModel(request, documentHeaderDto, billingsDtoList, contractDto);

                string template = template = RazorHelper.ParseTemplate(DocumemtTemplateUrl.Demand_Letter_1_3rd_Party, model);

                pdfBytes = PDFHelper.PdfSharpConvertBytes(template);

            }
            return pdfBytes;
        }

        private async Task<DocumentTemplateViewModel> GetTemplateViewModel(DocumentGeneratorViewModel request, DocumentHeaderDto documentHeaderDto, IEnumerable<RiskBillingDto> billingsDto, ContractDto contractDto)
        {
            DocumentTemplateViewModel model = DocumentGeneratorHelper.GetDocumentTemplateViewModel(documentHeaderDto);

            model.Contract = DocumentGeneratorHelper.MapContractViewModel(contractDto);

            model.Billings = DocumentGeneratorHelper.GetBillingViewModel(billingsDto);

            model.DateOfLoss = documentHeaderDto.DateOfLoss;

            model.TotalDue = model.Billings != null && model.Billings.Any() ? model.Billings.Sum(x => x.SubTotal) : default(double); 

            return model;
        }

       
    }
}