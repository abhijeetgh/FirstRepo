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
    public class VehicleRelease : IDocumentGenerator
    {
         IDocumentGeneratorService _documentGeneratorService=null;

         public VehicleRelease(IDocumentGeneratorService documentGeneratorService)
        {
            _documentGeneratorService = documentGeneratorService;
        }

        public async Task<byte[]> GetBytesAsync(ViewModels.DocumentGenerator.DocumentGeneratorViewModel request)
        {
            byte[] pdfBytes = null;

            if (request != null)
            {

                DocumentHeaderDto documentHeaderDto = await _documentGeneratorService.GetDocumentHeaderInfoAsync(request.ClaimId, request.SelectedDriverId);

                DocumentTemplateViewModel model = DocumentGeneratorHelper.GetDocumentTemplateViewModel(documentHeaderDto);

                string template = template = RazorHelper.ParseTemplate(DocumemtTemplateUrl.Vehicle_Release, model);

                pdfBytes = PDFHelper.PdfSharpConvertBytes(template);

            }
            return pdfBytes;
        }
    }
}