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
    public class RepossesionAutherizationNationwideRepo : IDocumentGenerator
    {

        IDocumentGeneratorService _documentGeneratorService = null;

        public RepossesionAutherizationNationwideRepo(IDocumentGeneratorService documentGeneratorService)
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

                model.Company = new CompanyViewModel
                {
                    Logopath = HostingEnvironment.MapPath("~/Images/repologo.gif")
                };

                string template = GetTemplate((DocumentTypes)request.DocumentTypeId, model);

                pdfBytes = PDFHelper.PdfSharpConvertBytes(template);

            }


            return pdfBytes;

        }

        private static string GetTemplate(DocumentTypes docType, DocumentTemplateViewModel model)
        {
            string template = string.Empty;
            if (docType == DocumentTypes.Repo_Auth_Nationwide_Repo)
            {

                template = RazorHelper.ParseTemplate(DocumemtTemplateUrl.Repo_Auth_Nationwide_Repo, model);
            }
            return template;
        }
    }
}