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
    public class ReposessionAuthorization : IDocumentGenerator
    {
        IDocumentGeneratorService _documentGeneratorService = null;

        public ReposessionAuthorization(IDocumentGeneratorService documentGeneratorService)
        {
            _documentGeneratorService = documentGeneratorService;
        }

        public async Task<byte[]> GetBytesAsync(DocumentGeneratorViewModel request)
        {

            byte[] pdfBytes = null;

            if (request != null)
            {
                RepoAuthorziationViewModel viewModel = new RepoAuthorziationViewModel();
                

                DocumentHeaderDto documentHeaderDto = await _documentGeneratorService.GetDocumentHeaderInfoAsync(request.ClaimId, request.SelectedDriverId);

                DocumentTemplateViewModel model = DocumentGeneratorHelper.GetDocumentTemplateViewModel(documentHeaderDto);

                viewModel.HeaderViewModel = model;

                var riskIncident = await _documentGeneratorService.GetIncidentByClaimIdAsync(request.ClaimId);

                viewModel.PolicyAgency = riskIncident.SelectedPoliceAgencyName;

                viewModel.CaseNumber = riskIncident.CaseNumber;
                

                string template = GetTemplate((DocumentTypes)request.DocumentTypeId, viewModel);

                pdfBytes = PDFHelper.PdfSharpConvertBytes(template);

            }


            return pdfBytes;

        }

        private static string GetTemplate(DocumentTypes docType, RepoAuthorziationViewModel model)
        {
            string template = string.Empty;
            if (docType == DocumentTypes.Reposession_Authorization)
            {

                template = RazorHelper.ParseTemplate(DocumemtTemplateUrl.Reposession_Authorization, model);
            }
            return template;
        }
    }
}