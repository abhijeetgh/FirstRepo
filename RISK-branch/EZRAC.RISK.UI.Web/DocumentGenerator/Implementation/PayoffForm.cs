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
    public class PayoffForm :IDocumentGenerator
    {
        IDocumentGeneratorService _documentGeneratorService = null;
        IClaimService _claimService = null;

        public PayoffForm(IDocumentGeneratorService documentGeneratorService, IClaimService claimService)
        {
            _documentGeneratorService = documentGeneratorService;
            _claimService = claimService;
        }

        public async Task<byte[]> GetBytesAsync(DocumentGeneratorViewModel request)
        {

            byte[] pdfBytes = null;

            if (request != null)
            {
                PayoffFormViewModel viewModel = new PayoffFormViewModel();

                DocumentHeaderDto documentHeaderDto = await _documentGeneratorService.GetDocumentHeaderInfoAsync(request.ClaimId, request.SelectedDriverId);

                DocumentTemplateViewModel model = DocumentGeneratorHelper.GetDocumentTemplateViewModel(documentHeaderDto);

                viewModel.HeaderViewModel = model;

                viewModel.HeaderViewModel.DateOfLoss = documentHeaderDto.DateOfLoss;

                viewModel.OpenDate = documentHeaderDto.OpenDate;

                var estimateAmount= await _documentGeneratorService.GetEstimatedBilling(request.ClaimId);
                viewModel.EstimatedAmount = Math.Round(estimateAmount, 2);

                ContractDto contractDto = await _claimService.GetContractByClaimIdAsync(request.ClaimId);
                viewModel.Contract = DocumentGeneratorHelper.MapContractViewModel(contractDto);

                viewModel.ActualCashValue = await _documentGeneratorService.GetActualCashValueForClaim(request.ClaimId);

                string template = GetTemplate((DocumentTypes)request.DocumentTypeId, viewModel);

                pdfBytes = PDFHelper.PdfSharpConvertBytes(template);

            }

            return pdfBytes;

        }

        private static string GetTemplate(DocumentTypes docType, PayoffFormViewModel model)
        {
            string template = string.Empty;
            if (docType == DocumentTypes.Payoff_Form)
            {

                template = RazorHelper.ParseTemplate(DocumemtTemplateUrl.Payoff_Form, model);
            }
            return template;
        }
    }
}