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
    public class ViolationAdminFee :IDocumentGenerator
    {
            IDocumentGeneratorService _documentGeneratorService = null;

            public ViolationAdminFee(IDocumentGeneratorService documentGeneratorService)
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

                    model.TreatAsCaliforniaTicket = request.TreatAsCaliforniaTicket;

                    string template = GetTemplate(request.DocumentTypeId, model);

                    pdfBytes = PDFHelper.PdfSharpConvertBytes(template);

                }


                return pdfBytes;

            }

            private static string GetTemplate(DocumentTypes docType, DocumentTemplateViewModel model)
            {
                string template = string.Empty;
                if (docType == DocumentTypes.Violation_Admin_Fee)
                {

                    template = RazorHelper.ParseTemplate(DocumemtTemplateUrl.Violation_Admin_Fee, model);
                }
                else
                {
                    template = RazorHelper.ParseTemplate(DocumemtTemplateUrl.Violation_Admin_Fee_Decline, model);
                }
                return template;
            }
        }
}