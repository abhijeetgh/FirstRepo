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
    public class RenterDemandLetterThreeAndFour : IDocumentGenerator
    {
         IDocumentGeneratorService _documentGeneratorService = null;
         IClaimService _claimService;
        

         public RenterDemandLetterThreeAndFour(IDocumentGeneratorService documentGeneratorService,IClaimService claimService)
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

                 ContractDto contractDto = await _claimService.GetContractByClaimIdAsync(request.ClaimId);

                 DocumentTemplateViewModel model = await GetTemplateViewModel(request, documentHeaderDto, contractDto);

                 string template = GetTemplate(request.DocumentTypeId, model);

                 pdfBytes = PDFHelper.PdfSharpConvertBytes(template);

             }
             return pdfBytes;
         }

         private async Task<DocumentTemplateViewModel> GetTemplateViewModel(DocumentGeneratorViewModel request, DocumentHeaderDto documentHeaderDto, ContractDto contractDto)
         {
             DocumentTemplateViewModel model = DocumentGeneratorHelper.GetDocumentTemplateViewModel(documentHeaderDto);

             model.Contract = DocumentGeneratorHelper.MapContractViewModel(contractDto);

             model.DateOfLoss = documentHeaderDto.DateOfLoss;

             model.TotalDue = await _documentGeneratorService.GetTotalDueAsync(request.SelectedBillings, request.ClaimId);

             return model;
         }

         private static string GetTemplate(DocumentTypes docType, DocumentTemplateViewModel model)
         {
             string template = string.Empty;
             if (docType == DocumentTypes.Demand_Letter_3_Renter)
             {

                 template = RazorHelper.ParseTemplate(DocumemtTemplateUrl.Demand_Letter_3_Renter, model);
             }
             else if (docType == DocumentTypes.Demand_Letter_4_Renter)
             {

                 template = RazorHelper.ParseTemplate(DocumemtTemplateUrl.Demand_Letter_4_Renter, model);
             }


             return template;
         }
    }
}