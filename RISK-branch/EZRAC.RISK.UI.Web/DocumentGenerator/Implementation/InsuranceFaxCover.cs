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
    public class InsuranceFaxCover : IDocumentGenerator
    {
        IDocumentGeneratorService _documentGeneratorService = null;
        IClaimService _claimService;

        public InsuranceFaxCover(IDocumentGeneratorService documentGeneratorService, IClaimService claimService)
        {
            _documentGeneratorService = documentGeneratorService;
            _claimService = claimService;
           
        }

        public async Task<byte[]> GetBytesAsync(DocumentGeneratorViewModel request)
        {
            byte[] pdfBytes = null;

            if (request != null)
            {

                DocumentHeaderDto documentHeaderDto = await _documentGeneratorService.GetDocumentHeaderInfoAsync(request.ClaimId, request.SelectedDriverId);

                ContractDto contractDto = await _claimService.GetContractByClaimIdAsync(request.ClaimId);

                InsuranceDto insuranceDto = await _documentGeneratorService.GetInsuranceInfoByDriverId(request.SelectedDriverId);

                DocumentTemplateViewModel model = await GetTemplateViewModel(request, documentHeaderDto, contractDto, insuranceDto);

                string template = RazorHelper.ParseTemplate(DocumemtTemplateUrl.Insurance_Fax_Cover_Sheet, model);

                pdfBytes = PDFHelper.PdfSharpConvertBytes(template);

            }
            return pdfBytes;
        }

        private async Task<DocumentTemplateViewModel> GetTemplateViewModel(DocumentGeneratorViewModel request, DocumentHeaderDto documentHeaderDto,
                                               ContractDto contractDto, InsuranceDto insuranceDto)
        {
            DocumentTemplateViewModel model = DocumentGeneratorHelper.GetDocumentTemplateViewModel(documentHeaderDto);

            model.DriverViewModel.InsuranceViewModel = DocumentGeneratorHelper.MapInsuranceViewModel(insuranceDto);

            model.Contract = DocumentGeneratorHelper.MapContractViewModel(contractDto);

            //model.DateOfLoss = await _documentGeneratorService.GetDateOfLossAsync(request.ClaimId);

            //model.VehicleViewModel = new VehicleViewModel();

           // model.VehicleViewModel.UnitNumber = await _documentGeneratorService.GetUnitNumberAsync(request.ClaimId);

           // model.TotalDue = await _documentGeneratorService.GetTotalDueAsync(request.SelectedBillings, request.ClaimId);

            return model;
        }
    }
}