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
    public class TicketAffidavit : IDocumentGenerator
    {
            IDocumentGeneratorService _documentGeneratorService = null;
            IClaimService _claimService = null;

            public TicketAffidavit(IDocumentGeneratorService documentGeneratorService, IClaimService claimService)
            {
                _documentGeneratorService = documentGeneratorService;
                _claimService = claimService;
            }

            public async Task<byte[]> GetBytesAsync(DocumentGeneratorViewModel request)
            {

                byte[] pdfBytes = null;

                if (request != null)
                {
                    TicketAffidavitViewModel viewModel = new TicketAffidavitViewModel();

                    DocumentHeaderDto documentHeaderDto = await _documentGeneratorService.GetDocumentHeaderInfoAsync(request.ClaimId, request.SelectedDriverId);

                    DocumentTemplateViewModel model = DocumentGeneratorHelper.GetDocumentTemplateViewModel(documentHeaderDto);

                    ContractDto contractDto = await _claimService.GetContractByClaimIdAsync(request.ClaimId);
                    model.Contract = DocumentGeneratorHelper.MapContractViewModel(contractDto);

                    viewModel.HeaderViewModel = model;

                    string template = GetTemplate(request.DocumentTypeId, viewModel);

                    pdfBytes = PDFHelper.PdfSharpConvertBytes(template);

                }


                return pdfBytes;

            }

            private static string GetTemplate(DocumentTypes docType, TicketAffidavitViewModel model)
            {
                string template = string.Empty;
                if (docType == DocumentTypes.Ticket_Affidavit)
                {

                    template = RazorHelper.ParseTemplate(DocumemtTemplateUrl.Ticket_Affidavit, model);
                }
                return template;
            }
        }
}