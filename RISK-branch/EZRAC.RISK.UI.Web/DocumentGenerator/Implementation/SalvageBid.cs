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
    public class SalvageBid : IDocumentGenerator
    {
            IDocumentGeneratorService _documentGeneratorService = null;

            public SalvageBid(IDocumentGeneratorService documentGeneratorService)
            {
                _documentGeneratorService = documentGeneratorService;
            }

            public async Task<byte[]> GetBytesAsync(DocumentGeneratorViewModel request)
            {
                byte[] pdfBytes = null;

                if (request != null)
                {
                    SalvageBidViewModel viewModel = new SalvageBidViewModel();
                    DocumentHeaderDto documentHeaderDto = await _documentGeneratorService.GetDocumentHeaderInfoAsync(request.ClaimId, request.SelectedDriverId);
                    DocumentTemplateViewModel model = DocumentGeneratorHelper.GetDocumentTemplateViewModel(documentHeaderDto);

                  
                    viewModel.HeaderViewModel = model;
                    var salvageAmount = request.SalvageBidAmount.HasValue ? request.SalvageBidAmount.Value : default(double);
                    viewModel.SalvageAmount = Math.Round(salvageAmount, 2);

                    string template = GetTemplate(request.DocumentTypeId, viewModel);

                    pdfBytes = PDFHelper.PdfSharpConvertBytes(template);

                }
                return pdfBytes;
            }

            private static string GetTemplate(DocumentTypes docType, SalvageBidViewModel model)
            {
                string template = string.Empty;
                if (docType == DocumentTypes.Salvage_Bid_Accepted)
                {
                    template = RazorHelper.ParseTemplate(DocumemtTemplateUrl.Salvage_Bid_Accepted, model);
                }
                else if (docType == DocumentTypes.Salvage_Bid)
                {
                    template = RazorHelper.ParseTemplate(DocumemtTemplateUrl.Salvage_Bid, model);
                }
                return template;
            }
        }
}