using EZRAC.Core.FileGenerator;
using EZRAC.Core.Util;
using EZRAC.Risk.UI.Web.Helper;
using EZRAC.Risk.UI.Web.ViewModels.DocumentGenerator;
using EZRAC.RISK.Services.Contracts;
using EZRAC.RISK.Services.Contracts.Dtos;
using EZRAC.RISK.Util.Common;
using System.Threading.Tasks;

namespace EZRAC.Risk.UI.Web.DocumentGenerator.Implementation
{
    public class ClientInfoAndAuthorization : IDocumentGenerator
    {
         IDocumentGeneratorService _documentGeneratorService = null;

         public ClientInfoAndAuthorization(IDocumentGeneratorService documentGeneratorService)
         {
             _documentGeneratorService = documentGeneratorService;
         }

        public async Task<byte[]> GetBytesAsync(DocumentGeneratorViewModel request)
        {
            byte[] pdfBytes = null;

            if (request != null)
            {

                DocumentHeaderDto documentHeaderDto = await _documentGeneratorService.GetDocumentHeaderInfoAsync(request.ClaimId, request.SelectedDriverId);

                DocumentTemplateViewModel model = DocumentGeneratorHelper.GetDocumentTemplateViewModel(documentHeaderDto);

                model.TotalDue = await _documentGeneratorService.GetTotalDueAsync(request.SelectedBillings, request.ClaimId);

                string template = template = RazorHelper.ParseTemplate(DocumemtTemplateUrl.Client_Info_and_Authorization, model);

                pdfBytes = PDFHelper.PdfSharpConvertBytes(template);

            }


            return pdfBytes;
        }
    }
}