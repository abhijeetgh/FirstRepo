using EZRAC.Core.FileGenerator;
using EZRAC.Core.Util;
using EZRAC.Risk.UI.Web.Helper;
using EZRAC.Risk.UI.Web.ViewModels.DocumentGenerator;
using EZRAC.RISK.Services.Contracts;
using EZRAC.RISK.Services.Contracts.Dtos;
using EZRAC.RISK.Util.Common;
using System;
using System.Threading.Tasks;

namespace EZRAC.Risk.UI.Web.DocumentGenerator.Implementation
{
    public class Ardl: IDocumentGenerator
    {
        IDocumentGeneratorService _documentGeneratorService=null;

        public Ardl(IDocumentGeneratorService documentGeneratorService)
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

                string template = GetTemplate(request.DocumentTypeId, model);

                pdfBytes = PDFHelper.PdfSharpConvertBytes(template);

            }

      
            return pdfBytes;
            
        }

        private static string GetTemplate(DocumentTypes docType, DocumentTemplateViewModel model)
        {
            string template = string.Empty;
            if (docType == DocumentTypes.AR_DL1_RTR)
            {

                template = RazorHelper.ParseTemplate(DocumemtTemplateUrl.AR_DL1_RTR, model);
            }
            else if (docType == DocumentTypes.AR_DL2_RTR)
            {

                template = RazorHelper.ParseTemplate(DocumemtTemplateUrl.AR_DL2_RTR, model);
            }
            else
            {
                template = RazorHelper.ParseTemplate(DocumemtTemplateUrl.AR_DL3_RTR, model);
            }

            return template;
        }
    }
}