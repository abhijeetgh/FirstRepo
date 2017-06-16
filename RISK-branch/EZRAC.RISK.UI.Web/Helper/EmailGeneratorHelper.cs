using EZRAC.Risk.UI.Web.ViewModels.Claims;
using EZRAC.Risk.UI.Web.ViewModels.EmailGenerator;
using EZRAC.RISK.Services.Contracts.Dtos;
using System.Collections.Generic;
using System;
using System.Linq;
using System.Web.Mvc;
using EZRAC.RISK.Util;

namespace EZRAC.Risk.UI.Web.Helper
{
    public class EmailGeneratorHelper
    {

        internal static EmailGeneratorViewModel GetEmailGeneratorViewModel(IEnumerable<string> riskUserEmails, IEnumerable<NotesDto> notes, 
                                                                           IEnumerable<PicturesAndFilesDto> files, 
                                                                           IEnumerable<InformationToShowDto> informationToSendDto,
                                                                           IEnumerable<EmailGeneratorReceipientDto> recipients)
        {
            EmailGeneratorViewModel emailGeneratorViewModel = new EmailGeneratorViewModel();

            emailGeneratorViewModel.InformationToSendList = GetInformationToSend(informationToSendDto);

            emailGeneratorViewModel.Files = GetFilesViewModel(files);

            emailGeneratorViewModel.Notes = ClaimHelper.GetNotesViewModel(notes);

            emailGeneratorViewModel.RiskUserEmails = GetRiskUserEmails(riskUserEmails);

            emailGeneratorViewModel.Recipients = GetRecipients(recipients);

            

            return emailGeneratorViewModel;
        }

        private static IEnumerable<InformationToShowViewModel> GetInformationToSend(IEnumerable<InformationToShowDto> informationToSendDto)
        {
            var informationToSendViewModel = informationToSendDto.Select(
                x => new InformationToShowViewModel
                {
                    Id = x.Id,
                    Text = x.Text,
                    IsAvailable = x.IsAvailable
                });

            return informationToSendViewModel;
        }

        private static IEnumerable<SelectListItem> GetRiskUserEmails(IEnumerable<string> riskUserEmails)
        {
            IEnumerable<SelectListItem> riskUserEmailList = null;
            if (riskUserEmails != null)
            {
                riskUserEmailList = riskUserEmails.Where(y => !String.IsNullOrEmpty(y)).Select(
                x => new SelectListItem
                {
                    Text = x,
                    Value = x
                }).ToList();
            }

            return riskUserEmailList;
        }

        private static IEnumerable<SelectListItem> GetRecipients(IEnumerable<EmailGeneratorReceipientDto> recipients)
        {
            List<SelectListItem> recipientList = recipients.Select(
                x => new SelectListItem
                {
                    Value = x.Value.ToString(),
                    Text = x.Text
                }).ToList();

            return recipientList;
        }

        private static IEnumerable<RiskFileModel> GetFilesViewModel(IEnumerable<PicturesAndFilesDto> files)
        {
            List<RiskFileModel> filesModel = null;

            if (files.Any())
            {
                filesModel = new List<RiskFileModel>();

                var groupByResult = files.GroupBy(x => x.CategoryName);

                foreach (var item in groupByResult)
                {
                    filesModel.Add(
                        new RiskFileModel
                        {
                            CategoryName = item.Key,
                            CategoryId = item.FirstOrDefault().CategoryId,
                            FileList = item.Select(x => new FileModel { Id = x.FileId, FileName = x.FileName }).ToList()
                        });

                }

            }

            return filesModel;
        }



        internal static EmailGeneratorDto GetEmailGeneratorDto(EmailGeneratorViewModel emailGeneratorViewModel, InformationToSendDto informationToSendDto,
                                                               IEnumerable<NotesDto> notes, IEnumerable<PicturesAndFilesDto> files, String CompanyAbbr, string userEmail,string subject)
        {
            EmailGeneratorDto emailGeneratorDto = null;

            if (emailGeneratorViewModel != null)
            {
                emailGeneratorDto = new EmailGeneratorDto();
                emailGeneratorDto.SelectedReceipient = emailGeneratorViewModel.SelectedRecipient;
                emailGeneratorDto.SelectedUserEmails = emailGeneratorViewModel.SelectedUserEmails;
                emailGeneratorDto.CustomEmailAddress = emailGeneratorViewModel.CustomEmailAddress;
                emailGeneratorDto.InformationToSendDto = informationToSendDto;
                emailGeneratorDto.SelectedNotes = notes;
                emailGeneratorDto.SelectedFiles = files;
                emailGeneratorDto.ClaimId = emailGeneratorViewModel.ClaimId;
                emailGeneratorDto.Remarks = emailGeneratorViewModel.Remarks;
                emailGeneratorDto.CompanyAbbr = CompanyAbbr;
                emailGeneratorDto.LoggedInUserEmail = userEmail;
                emailGeneratorDto.Subject = subject;

            }

            return emailGeneratorDto;

        }
    }
}