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
using System.Web.Hosting;

namespace EZRAC.Risk.UI.Web.DocumentGenerator.Implementation
{
    public class BodyShopRepair : IDocumentGenerator
    {
        IDocumentGeneratorService _documentGeneratorService=null;
        IClaimService _claimService = null;

        public BodyShopRepair(IDocumentGeneratorService documentGeneratorService, IClaimService claimService)
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

                DocumentTemplateViewModel model = DocumentGeneratorHelper.GetDocumentTemplateViewModel(documentHeaderDto);

                IEnumerable<DamageDto> claimForDamages = await _claimService.GetDamagesInfoByClaimIdAsync(request.ClaimId);
                if (model != null)
                {
                    model.Damages = ClaimHelper.GetDamagesViewModel(claimForDamages);
                }
                string template = template = RazorHelper.ParseTemplate(DocumemtTemplateUrl.Body_Shop_Repair, model);

                pdfBytes = PDFHelper.PdfSharpConvertBytes(template);

            }
            return pdfBytes;
        }
    }
}