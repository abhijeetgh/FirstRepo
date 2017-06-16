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
    public class JnrDebtorPlacementForm : IDocumentGenerator
    {
         IDocumentGeneratorService _documentGeneratorService=null;
       

        public JnrDebtorPlacementForm(IDocumentGeneratorService documentGeneratorService)
        {
            _documentGeneratorService = documentGeneratorService;
        }


        public async Task<byte[]> GetBytesAsync(DocumentGeneratorViewModel request)
        {

            byte[] pdfBytes = null;

            if (request != null)
            {
                DocumentHeaderDto documentHeaderDto = await _documentGeneratorService.GetDocumentHeaderInfoAsync(request.ClaimId, request.SelectedDriverId);

                double totalDue = await _documentGeneratorService.GetTotalDueAsync(request.SelectedBillings, request.ClaimId);

                DocumentTemplateViewModel model = DocumentGeneratorHelper.GetDocumentTemplateViewModel(documentHeaderDto);

                var dateOfLoss = documentHeaderDto.DateOfLoss;

                model.TotalDue = totalDue;
                model.DateOfLoss = dateOfLoss;
              
                model.DateOfLastPayment = await _documentGeneratorService.GetDateOfLastPaymentAsync(request.ClaimId);

                model.Company = new CompanyViewModel{
                    Logopath = HostingEnvironment.MapPath(String.Format("~/Images/jnrlogo.png"))
                };
                string template = GetTemplate((DocumentTypes)request.DocumentTypeId, model);

                pdfBytes = PDFHelper.PdfSharpConvertBytes(template);

            }

            return pdfBytes;

        }


        private static string GetTemplate(DocumentTypes docType, DocumentTemplateViewModel model)
        {
            string template = string.Empty;
            if (docType == DocumentTypes.JNR_Debtor_Placement_Form)
            {
                template = RazorHelper.ParseTemplate(DocumemtTemplateUrl.JNR_Debtor_Placement_Form, model);
            }
            return template;
        }
       
    }
}