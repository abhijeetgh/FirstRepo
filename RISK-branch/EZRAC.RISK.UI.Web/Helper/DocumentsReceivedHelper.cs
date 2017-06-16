using EZRAC.Risk.UI.Web.ViewModels.DocumentsReceived;
using EZRAC.RISK.Services.Contracts.Dtos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace EZRAC.Risk.UI.Web.Helper
{
    public class DocumentsReceivedHelper
    {

        internal static DocumentsReceivedViewModel GetDocumentsReceivedViewModel(DocumentsReceivedDto documentsReceivedDto)
        {
            DocumentsReceivedViewModel documentsReceivedViewModel = new DocumentsReceivedViewModel(); ;
            if (documentsReceivedDto != null)
            {
                
                documentsReceivedViewModel.ClaimId = documentsReceivedDto.ClaimId;
                documentsReceivedViewModel.ClaimFolder = documentsReceivedDto.ClaimFolder.HasValue ? documentsReceivedDto.ClaimFolder.Value : false;
                documentsReceivedViewModel.PoliceReport = documentsReceivedDto.PoliceReport.HasValue ? documentsReceivedDto.PoliceReport.Value : false;
                documentsReceivedViewModel.EstimateApproved = documentsReceivedDto.EstimateApproved.HasValue ? documentsReceivedDto.EstimateApproved.Value : false;
                documentsReceivedViewModel.EstimateReceived = documentsReceivedDto.EstimateReceived.HasValue ? documentsReceivedDto.EstimateReceived.Value : false;
            }
            return documentsReceivedViewModel;
        }

        internal static DocumentsReceivedDto GetDocumentsReceivedDto(DocumentsReceivedViewModel documentsReceivedViewModel)
        {
            DocumentsReceivedDto documentsReceivedDto =  new DocumentsReceivedDto();
            if (documentsReceivedViewModel != null)
            {
                documentsReceivedDto = new DocumentsReceivedDto();
                documentsReceivedDto.ClaimId = documentsReceivedViewModel.ClaimId;
                documentsReceivedDto.ClaimFolder = documentsReceivedViewModel.ClaimFolder;
                documentsReceivedDto.PoliceReport = documentsReceivedViewModel.PoliceReport;
                documentsReceivedDto.EstimateApproved = documentsReceivedViewModel.EstimateApproved;
                documentsReceivedDto.EstimateReceived = documentsReceivedViewModel.EstimateReceived;
            }
            return documentsReceivedDto;
        }
    }
}