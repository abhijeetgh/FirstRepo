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
    public class NationalCasualtyInsuranceCover :IDocumentGenerator
    {
         IDocumentGeneratorService _documentGeneratorService=null;
       
        public NationalCasualtyInsuranceCover(IDocumentGeneratorService documentGeneratorService)
        {
            _documentGeneratorService = documentGeneratorService;
           
        }

        public async Task<byte[]> GetBytesAsync(DocumentGeneratorViewModel request)
        {

            byte[] pdfBytes = null;

            if (request != null)
            {

                DocumentHeaderDto documentHeaderDto = await _documentGeneratorService.GetDocumentHeaderInfoAsync(request.ClaimId, request.SelectedDriverId);

                NationalCasualtyInsuranceCoverViewModel  insuranceCoverViewModel = new NationalCasualtyInsuranceCoverViewModel();

                insuranceCoverViewModel.HeaderViewModel = DocumentGeneratorHelper.GetDocumentTemplateViewModel(documentHeaderDto);

                var driversDto = await _documentGeneratorService.GetDriversByClaimAsync(request.ClaimId);

                insuranceCoverViewModel.PolicyNumber = await _documentGeneratorService.GetPolicyNumberByDriverId(request.SelectedDriverId);

                insuranceCoverViewModel.Drivers = MapDriversDto(driversDto);

                insuranceCoverViewModel.HeaderViewModel.DateOfLoss = documentHeaderDto.DateOfLoss;

                if (request.DocumentTypeId == DocumentTypes.SLI_Fax_Cover_Sheet)
                {
                    insuranceCoverViewModel.PolicyNumber = await _documentGeneratorService.GetSliPolicyNumberByClaimId(request.ClaimId);
                }

                string template = GetTemplate((DocumentTypes)request.DocumentTypeId, insuranceCoverViewModel);

                pdfBytes = PDFHelper.PdfSharpConvertBytes(template);

            }

      
            return pdfBytes;
            
        }

        private static List<DriverViewModel> MapDriversDto(IEnumerable<DriverDto> driversDto)
        {
            List<DriverViewModel> listDrivers = new List<DriverViewModel>();
            DriverViewModel viewModel = null;
            foreach (var item in driversDto)
            {
                viewModel = new DriverViewModel();
                viewModel.Address = item.Address;
                viewModel.DriverType = Convert.ToInt32(item.DriverTypeId);
                viewModel.Name = item.FirstName + " " + item.LastName;
                viewModel.InsuranceViewModel = new InsuranceViewModel();
                viewModel.InsuranceViewModel.InsuranceCompanyName = item.InsuranceCompany;
                listDrivers.Add(viewModel);                
            }
            return listDrivers;
        }

        private static string GetTemplate(DocumentTypes docType, NationalCasualtyInsuranceCoverViewModel model)
        {
            string template = string.Empty;
            if (docType == DocumentTypes.National_Casualty_Insurance_Cover)
            {

                template = RazorHelper.ParseTemplate(DocumemtTemplateUrl.National_Casualty_Insurance_Cover, model);
            }
            else if(docType == DocumentTypes.SLI_Fax_Cover_Sheet) {

                template = RazorHelper.ParseTemplate(DocumemtTemplateUrl.SLI_Fax_Cover_Sheet, model);
            }
            else
            {
                template = RazorHelper.ParseTemplate(DocumemtTemplateUrl.Zurich_Empire_Letter, model);
            }
            return template;
        }

    }
}