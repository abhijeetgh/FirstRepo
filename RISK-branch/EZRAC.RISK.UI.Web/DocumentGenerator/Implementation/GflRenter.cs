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
    public class GflRenter : IDocumentGenerator
    {
        IDocumentGeneratorService _documentGeneratorService = null;
         IClaimService _claimService;
         

         public GflRenter(IDocumentGeneratorService documentGeneratorService, IClaimService claimService)
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

                 string template = RazorHelper.ParseTemplate(DocumemtTemplateUrl.GFL_Renter, model);

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

    }
}