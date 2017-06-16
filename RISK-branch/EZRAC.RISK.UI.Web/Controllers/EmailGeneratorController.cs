using EZRAC.Core.Email;
using EZRAC.Risk.UI.Web.Helper;
using EZRAC.Risk.UI.Web.ViewModels.EmailGenerator;
using EZRAC.RISK.Services.Contracts;
using EZRAC.RISK.Services.Contracts.Dtos;
using EZRAC.RISK.Util.Common;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace EZRAC.Risk.UI.Web.Controllers
{
    public class EmailGeneratorController : Controller
    {
       
        private INotesService _notesService = null;
        private IRiskFileService _riskFileService = null;
        private IEmailGeneratorService _emailGeneratorService = null;
        private IClaimService _claimService = null;
        private IUserService _userService = null;

        public EmailGeneratorController(INotesService notesService, 
                                        IRiskFileService riskFileService, 
                                        IEmailGeneratorService emailGeneratorService,
                                        IClaimService claimService, 
                                        IUserService userService)
        {
           
            _notesService = notesService;
            _riskFileService = riskFileService;
            _emailGeneratorService = emailGeneratorService;
            _claimService = claimService;
            _userService = userService;

        }

        //
        // GET: /EmailGenerator/
        public async Task<PartialViewResult> Index(long claimId)
        {
            IEnumerable<string> riskUserEmails = LookUpHelpers.GetAllUserEmails();

            IEnumerable<NotesDto> notes = await _notesService.GetNotesAsync(claimId, ClaimsConstant.GetNotesType.AllNotes);

            IEnumerable<PicturesAndFilesDto> files = await _riskFileService.GetFilesByClaimIdAsync(claimId);

            IEnumerable<InformationToShowDto> InfoToSend = await _emailGeneratorService.GetInformationToShowList(claimId);

            IEnumerable<EmailGeneratorReceipientDto> recipients = await _emailGeneratorService.GetRecipients(claimId);

            EmailGeneratorViewModel emailGeneratorViewModel = EmailGeneratorHelper.GetEmailGeneratorViewModel(riskUserEmails, notes, files, InfoToSend, recipients);

            emailGeneratorViewModel.ClaimId = claimId;

            return PartialView(emailGeneratorViewModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [CRMSAuthroize(ClaimType = ClaimsConstant.EmailGenerator)]
        public async Task<ActionResult> SendEmail(EmailGeneratorViewModel emailGeneratorViewModel)
        {

            bool emailStatus = false;
            if (ModelState.IsValid)
            {
                InformationToSendDto informationToSendDto = await _emailGeneratorService.GetInformationToSend(emailGeneratorViewModel.SelectedInfoToSend,
                                                                                                          emailGeneratorViewModel.ClaimId);

                IEnumerable<NotesDto> notes = await _notesService.GetNotesByNoteIds(emailGeneratorViewModel.SelectedNotesToSend);

                IEnumerable<PicturesAndFilesDto> files = await _riskFileService.GetFilesByFileIds(emailGeneratorViewModel.SelectedFilesToSend);

                String CompanyAbbr = (await _claimService.GetCompanyByClaimIdAsync(emailGeneratorViewModel.ClaimId)).Abbr;

                string subject = await _emailGeneratorService.GetSubjectLine(emailGeneratorViewModel.ClaimId, SecurityHelper.GetUserNameFromContext());

                var user = await _userService.GetUserbyIdAsync(SecurityHelper.GetUserIdFromContext());

                var userEmail = user != null ? user.Email : string.Empty;

                EmailGeneratorDto emailGeneratorDto = EmailGeneratorHelper.GetEmailGeneratorDto(emailGeneratorViewModel, informationToSendDto, notes, files, CompanyAbbr, userEmail,subject);

                emailGeneratorDto.EmailRecipients = await _emailGeneratorService.GetEmailRecipients(emailGeneratorDto);

                emailStatus = await SendEmail(emailGeneratorDto);
            }

            return Json(emailStatus, JsonRequestBehavior.AllowGet);
        }
        /// <summary>
        /// Generates Email Template and sends email
        /// </summary>
        /// <param name="emailGeneratorDto"></param>
        /// <returns></returns>
        private static async Task<bool> SendEmail(EmailGeneratorDto emailGeneratorDto)
        {
            bool emailStatus = false;

            EmailRequest emailTemplate = EmailTemplatesHelper.CreateEmailForEmailGenerator(emailGeneratorDto);
            
            if (emailTemplate != null)
            {
               
                //Sending Email
                emailStatus = await EmailHelper.SendEmailAsync(EmailHelper.CreateEmailMessage(emailTemplate));
            }
            return emailStatus;
        }
	}
}