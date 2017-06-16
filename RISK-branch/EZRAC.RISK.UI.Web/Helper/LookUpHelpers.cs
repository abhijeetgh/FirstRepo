using EZRAC.Core.Caching;
using EZRAC.RISK.Services.Contracts;
using EZRAC.RISK.Services.Contracts.Dtos;
using EZRAC.RISK.Services.Contracts.ReportDtos;
using EZRAC.RISK.Util.Common;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web.Mvc;

namespace EZRAC.Risk.UI.Web.Helper
{
    public static class LookUpHelpers
    {
        private static ILookUpService _lookUpService = UnityResolver.ResolveService<ILookUpService>();
        private static IRiskReportDefaultSelectionService _riskReportDefaultSelectionService = UnityResolver.ResolveService<IRiskReportDefaultSelectionService>();

        //internal static List<SelectListItem> GetSelectListItems(Dictionary<int, string> lookupValues)
        //{

        //    List<SelectListItem> lookupValueResult = new List<SelectListItem>();

        //    if (lookupValues != null && lookupValues.Count > 0)
        //    {
        //        lookupValueResult = lookupValues.Select(x =>
        //           new SelectListItem
        //           {
        //               Text = x.Value,
        //               Value = x.Key.ToString(),
        //           }).ToList();
        //    }

        //    return lookupValueResult.OrderBy(x => x.Text).ToList();
        //}

        /// <summary>
        /// This method is to get Assigned Users list
        /// </summary>
        /// <returns></returns>
        internal static List<SelectListItem> GetAssignedToUsersListItem()
        {
            List<SelectListItem> lookupValueResult = new List<SelectListItem>();

            IEnumerable<UserDto> userList = Cache.Get<List<UserDto>>(Constants.CacheConstants.AssignedUsers);

            if (userList == null)
            {
                userList = _lookUpService.GetAssignToUsersAsync().Result;

                Cache.Add(Constants.CacheConstants.AssignedUsers, userList);
            }

            if (userList != null)
            {
                lookupValueResult = userList.Select(x =>
                   new SelectListItem
                   {
                       Text = string.Format("{0} {1}", x.FirstName, x.LastName),
                       Value = x.Id.ToString(),
                   }).OrderBy(x => x.Text).ToList();
            }

            return lookupValueResult;
        }

        /// <summary>
        /// This method is used to get All Locations
        /// </summary>
        /// <returns></returns>
        internal static List<SelectListItem> GetLocationListItem()
        {
            List<SelectListItem> lookupValueResult = new List<SelectListItem>();

            IEnumerable<LocationDto> locationList = Cache.Get<List<LocationDto>>(Constants.CacheConstants.Locations);

            if (locationList == null)
            {
                locationList = _lookUpService.GetAllLocationsAsync().Result;
                Cache.Add(Constants.CacheConstants.Locations, locationList);
            }
            if (locationList != null)
            {
                lookupValueResult = locationList.Select(x =>
                   new SelectListItem
                   {
                       Text = String.Format("{0} {1}", x.Name, x.CompanyAbbr != null ? x.CompanyAbbr.ToUpper() : String.Empty),
                       Value = x.Id.ToString(),
                   }).OrderBy(x => x.Text).ToList();
            }
            return lookupValueResult;
        }

        /// <summary>
        /// This method is used to get LoosTypes
        /// </summary>
        /// <returns></returns>
        internal static List<SelectListItem> GetLossTypeListItems()
        {
            List<SelectListItem> lookupValueResult = new List<SelectListItem>();

            IEnumerable<LossTypesDto> lossTypes = Cache.Get<List<LossTypesDto>>(Constants.CacheConstants.LossTypes);

            if (lossTypes == null)
            {
                lossTypes = _lookUpService.GetAllLossTypesAsync().Result;
                Cache.Add(Constants.CacheConstants.LossTypes, lossTypes);
            }

            if (lossTypes != null)
            {
                lookupValueResult = lossTypes.Select(x =>
                   new SelectListItem
                   {
                       Text = x.Description,
                       Value = x.Id.ToString(),
                   }).OrderBy(x => x.Text).ToList();
            }
            return lookupValueResult;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="users"></param>
        /// <returns></returns>
        internal static List<SelectListItem> GetSelectListItems(IEnumerable<UserDto> users)
        {
            List<SelectListItem> lookupValueResult = new List<SelectListItem>();

            if (users != null)
            {
                lookupValueResult = users.Select(x =>
                   new SelectListItem
                   {
                       Text = string.Format("{0} {1}", x.FirstName, x.LastName),
                       Value = x.Id.ToString(),
                   }).ToList();
            }

            return lookupValueResult.OrderBy(x => x.Text).ToList();
        }


        /// <summary>
        /// This method is used to get  all Claim Status List
        /// </summary>
        /// <returns></returns>
        internal static List<SelectListItem> GetClaimStatusListItems()
        {
            List<SelectListItem> lookupValueResult = new List<SelectListItem>();

            IEnumerable<ClaimStatusDto> claimStatusList = Cache.Get<List<ClaimStatusDto>>(Constants.CacheConstants.ClaimStatuses);

            if (claimStatusList == null)
            {
                claimStatusList = _lookUpService.GetAllClaimStatusesAsync().Result;
                Cache.Add(Constants.CacheConstants.ClaimStatuses, claimStatusList);
            }

            if (claimStatusList != null)
            {
                lookupValueResult = claimStatusList.Select(x =>
                   new SelectListItem
                   {
                       Text = x.Description,
                       Value = x.Id.ToString(),
                   }).OrderBy(x => x.Text).ToList();
            }

            return lookupValueResult;
        }


        /// <summary>
        /// This method is used to get AllNotes 
        /// </summary>
        /// <returns></returns>
        internal static List<SelectListItem> GetAllNotesListItem()
        {
            var lookupValueResult = new List<SelectListItem>();

            IEnumerable<NotesTypeDto> noteTypes = Cache.Get<List<NotesTypeDto>>(Constants.CacheConstants.NoteTypes);

            if (noteTypes == null)
            {
                noteTypes = _lookUpService.GetAllNoteTypesAsync().Result;
                Cache.Add(Constants.CacheConstants.NoteTypes, noteTypes);
            }

            if (noteTypes != null)
            {
                lookupValueResult = noteTypes.Select(x =>
                    new SelectListItem()
                    {
                        Text = x.Description,
                        Value = x.Id.ToString()
                    }).OrderBy(x => x.Text).ToList();
            }
            return lookupValueResult;
        }

        /// <summary>
        /// This method is used to get All police agencies
        /// </summary>
        /// <returns></returns>
        internal static List<SelectListItem> GetAllPoliceAgenciesListItem()
        {
            List<SelectListItem> lookupValueResult = new List<SelectListItem>();

            IEnumerable<PoliceAgencyDto> policeAgencies = Cache.Get<List<PoliceAgencyDto>>(Constants.CacheConstants.PoliceAgenecies);

            if (policeAgencies == null)
            {
                policeAgencies = _lookUpService.GetAllPoliceAgenciesAsync().Result;
                Cache.Add(Constants.CacheConstants.PoliceAgenecies, policeAgencies);

            }
            if (policeAgencies != null)
            {
                lookupValueResult = policeAgencies.Select(x =>
                   new SelectListItem
                   {
                       Text = x.AgencyName,
                       Value = x.Id.ToString(),
                   }).OrderBy(x => x.Text).ToList();
            }

            return lookupValueResult;
        }

        /// <summary>
        /// This method is used to get All DamageTypes
        /// </summary>
        /// <returns></returns>
        internal static List<SelectListItem> GetAllDamageTypesListItem()
        {
            List<SelectListItem> lookupValueResult = new List<SelectListItem>();

            IEnumerable<DamageTypesDto> damageTypes = Cache.Get<List<DamageTypesDto>>(Constants.CacheConstants.DamageTypes);

            if (damageTypes == null)
            {
                damageTypes = _lookUpService.GetAllDamageTypesAsync().Result;
                Cache.Add(Constants.CacheConstants.DamageTypes, damageTypes);
            }

            if (damageTypes != null)
            {
                lookupValueResult = damageTypes.Select(x =>
                      new SelectListItem
                      {
                          Text = x.Section,
                          Value = x.Id.ToString(),
                      }).OrderBy(x => x.Text).ToList();
            }
            return lookupValueResult;
        }

        /// <summary>
        /// This method is used to get All Insurance companies
        /// </summary>
        /// <returns></returns>
        internal static List<SelectListItem> GetAllInsuranceComapanyListItems()
        {
            List<SelectListItem> lookupValueResult = new List<SelectListItem>();

            IEnumerable<InsuranceDto> insuranceCompanies = Cache.Get<List<InsuranceDto>>(Constants.CacheConstants.InsuranceCompanies);

            if (insuranceCompanies == null)
            {
                insuranceCompanies = _lookUpService.GetAllInsuranceCompaniesAsync().Result;
                Cache.Add(Constants.CacheConstants.InsuranceCompanies, insuranceCompanies);
            }
            if (insuranceCompanies != null)
            {

                lookupValueResult = insuranceCompanies.Select(x =>
                      new SelectListItem
                      {
                          Text = x.CompanyName,
                          Value = x.Id.ToString(),
                      }).OrderBy(x => x.Text).ToList();
            }

            return lookupValueResult;
        }

        /// <summary>
        /// This method is used to get all FileCategories
        /// </summary>
        /// <returns></returns>
        internal static List<SelectListItem> GetFileCategoryListItems()
        {
            List<SelectListItem> lookupValueResult = null;

            IEnumerable<FileCategoriesDto> fileCategories = Cache.Get<List<FileCategoriesDto>>(Constants.CacheConstants.FileCategories);

            if (fileCategories == null)
            {
                fileCategories = _lookUpService.GetAllFileCategoriesAsync().Result;
                Cache.Add(Constants.CacheConstants.FileCategories, fileCategories);
            }
            if (fileCategories != null)
            {

                lookupValueResult = fileCategories.Select(x =>
                      new SelectListItem
                      {
                          Text = x.Description,
                          Value = x.Id.ToString(),
                      }).OrderBy(x => x.Text).ToList();
            }
            return lookupValueResult;
        }

        /// <summary>
        /// This method is used to get AllBillingTypes
        /// </summary>
        /// <returns></returns>
        internal static IList<SelectListItem> GetAllBillingTypes()
        {
            List<SelectListItem> lookupValueResult = null;

            IEnumerable<BillingTypeDto> billingTypes = Cache.Get<List<BillingTypeDto>>(Constants.CacheConstants.BilingTypes);

            if (billingTypes == null)
            {
                billingTypes = _lookUpService.GetBillingTypesAsync().Result;
                Cache.Add(Constants.CacheConstants.BilingTypes, billingTypes);
            }
            if (billingTypes != null)
            {

                lookupValueResult = billingTypes.Select(x =>
                      new SelectListItem
                      {
                          Text = x.Description,
                          Value = x.Id.ToString(),
                      }).OrderBy(x => x.Text).ToList();
            }

            return lookupValueResult;

        }

        /// <summary>
        /// This method is used to get AllWriteOffTypes
        /// </summary>
        /// <returns></returns>
        internal static IList<SelectListItem> GetAllWriteOffTypes()
        {
            List<SelectListItem> lookupValueResult = null;

            IEnumerable<WriteOffTypeDTO> writeOffTypes = Cache.Get<List<WriteOffTypeDTO>>(Constants.CacheConstants.WriteOffTypes);

            if (writeOffTypes == null)
            {
                writeOffTypes = _lookUpService.GetAllWriteOffTypesAsync().Result;
                Cache.Add(Constants.CacheConstants.WriteOffTypes, writeOffTypes);
            }
            if (writeOffTypes != null)
            {

                lookupValueResult = writeOffTypes.Select(x =>
                      new SelectListItem
                      {
                          Text = x.Type,
                          Value = x.Id.ToString(),
                      }).OrderBy(x => x.Text).ToList();
            }

            return lookupValueResult;

        }

        /// <summary>
        /// This method is used to get AllBillingTypes
        /// </summary>
        /// <returns></returns>
        internal static IList<SelectListItem> GetAllPaymentTypes()
        {
            List<SelectListItem> lookupValueResult = null;

            IEnumerable<PaymentTypesDto> PaymentTypes = Cache.Get<List<PaymentTypesDto>>(Constants.CacheConstants.PaymentType);

            if (PaymentTypes == null)
            {
                PaymentTypes = _lookUpService.GetAllPaymentTypesAsync().Result;
                Cache.Add(Constants.CacheConstants.PaymentType, PaymentTypes);
            }
            if (PaymentTypes != null)
            {

                lookupValueResult = PaymentTypes.Select(x =>
                      new SelectListItem
                      {
                          Text = x.PaymentType,
                          Value = x.Id.ToString(),
                      }).OrderBy(x => x.Text).ToList();
            }

            return lookupValueResult;

        }




        internal static List<SelectListItem> GetUserRoleItems(IEnumerable<UserRoleDto> usersRole, long selectedUserID)
        {
            List<SelectListItem> lookupValueResult = new List<SelectListItem>();

            if (usersRole != null)
            {
                lookupValueResult = usersRole.Select(x =>
                   new SelectListItem
                   {
                       Selected = (x.Id == selectedUserID),
                       Text = x.Name,
                       Value = Convert.ToString(x.Id),
                   }).ToList();
            }

            return lookupValueResult.OrderBy(x => x.Text).ToList();
        }

        internal static List<SelectListItem> GetCompanyListItem()
        {
            List<SelectListItem> lookupValueResult = null;

            IEnumerable<CompanyDto> companies = Cache.Get<List<CompanyDto>>(Constants.CacheConstants.Companies);

            if (companies == null)
            {
                companies = _lookUpService.GetAllCompaniesAsync().Result;
                Cache.Add(Constants.CacheConstants.Companies, companies);
            }
            if (companies != null)
            {

                lookupValueResult = companies.Select(x =>
                      new SelectListItem
                      {
                          Text = x.Name,
                          Value = x.Id.ToString(),
                      }).OrderBy(x => x.Text).ToList();
            }
            return lookupValueResult;
        }

        internal static IEnumerable<string> GetAllUserEmails()
        {

            IEnumerable<string> emailList = null;

            List<string> emails = Cache.Get<List<string>>(Constants.CacheConstants.AllUsersEmails);

            if (emails == null)
            {
                emailList = _lookUpService.GetAllUserEmailsAsync().Result;

                if (emailList != null && emailList.Any())
                {
                    Cache.Add(Constants.CacheConstants.AllUsersEmails, emailList.ToList());
                    emails = emailList.ToList();
                }
            }

            return emails;

        }

        internal static List<SelectListItem> GetAllUserListItem()
        {
            List<SelectListItem> lookupValueResult = new List<SelectListItem>();

            IEnumerable<UserDto> userList = Cache.Get<List<UserDto>>(Constants.CacheConstants.Users);

            if (userList == null)
            {
                userList = _lookUpService.GetAllUsersAsync().Result;
                Cache.Add(Constants.CacheConstants.Users, userList);
            }
            if (userList != null)
            {
                lookupValueResult = userList.Select(x =>
                   new SelectListItem
                   {
                       Text = String.Format("{0} {1}", Convert.ToString(x.FirstName), Convert.ToString(x.LastName)),
                       Value = x.Id.ToString(),
                   }).OrderBy(x => x.Text).ToList();
            }
            return lookupValueResult;
        }

        internal static String GetEmailTemplate(string emailTemplateFileName)
        {
            string emailMessageBody = String.Empty;
            string emailTemplateFilePath = String.Empty;

            emailMessageBody = Cache.Get<string>(emailTemplateFileName);

            if (String.IsNullOrEmpty(emailMessageBody))
            {
                if (string.IsNullOrWhiteSpace(emailTemplateFilePath))
                {
                    emailTemplateFilePath = String.Concat(System.Web.HttpContext.Current.Server.MapPath(ConfigSettingsReader.GetAppSettingValue(EmailConstants.AppSettings.EmailTemplatePath)), emailTemplateFileName);

                }

                if (File.Exists(emailTemplateFilePath))
                {
                    StreamReader templateReader = File.OpenText(emailTemplateFilePath);
                    try
                    {
                        if (templateReader != null)
                        {
                            emailMessageBody = templateReader.ReadToEnd();

                            Cache.Add(emailTemplateFileName, emailMessageBody);

                        }
                    }
                    finally
                    {
                        templateReader.Close();
                    }
                }
            }

            return emailMessageBody;

        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="drivers"></param>
        /// <returns></returns>
        internal static List<SelectListItem> GetSelectListItems(IEnumerable<DriverDto> drivers)
        {
            List<SelectListItem> lookupValueResult = new List<SelectListItem>();
            drivers = drivers.OrderBy(x => x.DriverTypeId);
            if (drivers != null)
            {
                lookupValueResult = drivers.Select(x =>
                   new SelectListItem
                   {
                       Text = string.Format("{0} - {1}", x.FirstName + " " + x.LastName, ClaimHelper.GetDriverTypeText(x.DriverTypeId)),
                       Value = x.DriverId.ToString(),
                   }).ToList();
            }

            return lookupValueResult.ToList();
        }

        internal static List<SelectListItem> GetAllDateTypes()
        {
            List<SelectListItem> lookupValueResult = new List<SelectListItem>();
            var DateTypeList = ReportsConstanct.DateTypes();
            if (DateTypeList != null)
            {
                lookupValueResult = DateTypeList.Select(x =>
                   new SelectListItem
                   {
                       Text = x.Value,
                       Value = x.Key.ToString(),
                   }).ToList();
            }
            return lookupValueResult;
        }
        internal static List<SelectListItem> GetAllReportTypes()
        {
            List<SelectListItem> lookupValueResult = new List<SelectListItem>();
            var ReportTypeList = ReportsConstanct.ReportTypes();
            if (ReportTypeList != null)
            {
                lookupValueResult = ReportTypeList.Select(x =>
                   new SelectListItem
                   {
                       Text = x.Value,
                       Value = x.Key.ToString(),
                   }).ToList();
            }
            return lookupValueResult;
        }

        internal static List<SelectListItem> GetAgingDays()
        {
            List<SelectListItem> lookupValueResult = new List<SelectListItem>();
            lookupValueResult.Add(new SelectListItem() { Text = "All", Value = "All", Selected = true });
            lookupValueResult.Add(new SelectListItem() { Text = "1-30", Value = "1-30" });
            lookupValueResult.Add(new SelectListItem() { Text = "31-60", Value = "31-60" });
            lookupValueResult.Add(new SelectListItem() { Text = "61-90", Value = "61-90" });
            lookupValueResult.Add(new SelectListItem() { Text = "91-120", Value = "91-120" });
            lookupValueResult.Add(new SelectListItem() { Text = "120+", Value = "120+" });
            return lookupValueResult;
        }

        /// <summary>
        /// Report Section auto selected drop down property
        /// </summary>
        /// <returns></returns>
        internal static List<SelectListItem> GetReportLocationListItem(string reportKey)
        {
            List<SelectListItem> lookupValueResult = new List<SelectListItem>();

            IEnumerable<LocationDto> locationList = Cache.Get<List<LocationDto>>(Constants.CacheConstants.Locations);
            IEnumerable<RiskReportDefaultSelectionDto> riskReportDefaultSelectionDto = _riskReportDefaultSelectionService.GetReportDefaultSelectionAsync(reportKey, ReportConstants.DefaultSelectionKey.Loc).ToList();
            if (locationList == null)
            {
                locationList = _lookUpService.GetAllLocationsAsync().Result;
                Cache.Add(Constants.CacheConstants.Locations, locationList);
            }
            if (locationList != null)
            {
                lookupValueResult = locationList.Select(x =>
                   new SelectListItem
                   {
                       Text = String.Format("{0} {1}", x.Name, x.CompanyAbbr != null ? x.CompanyAbbr.ToUpper() : String.Empty),
                       Value = x.Id.ToString(),
                       Selected = ((riskReportDefaultSelectionDto.Any() && riskReportDefaultSelectionDto.Where(y => y.PropId.Equals(x.Id)).Any()) ? true : false)
                   }).OrderBy(x => x.Text).ToList();
            }
            return lookupValueResult;
        }
        internal static List<SelectListItem> GetReportClaimStatusListItems(string reportkey)
        {
            List<SelectListItem> lookupValueResult = new List<SelectListItem>();

            IEnumerable<ClaimStatusDto> claimStatusList = Cache.Get<List<ClaimStatusDto>>(Constants.CacheConstants.ClaimStatuses);

            IEnumerable<RiskReportDefaultSelectionDto> riskReportDefaultSelectionDto = _riskReportDefaultSelectionService.GetReportDefaultSelectionAsync(reportkey, ReportConstants.DefaultSelectionKey.Status).ToList();

            if (claimStatusList == null)
            {
                claimStatusList = _lookUpService.GetAllClaimStatusesAsync().Result;
                Cache.Add(Constants.CacheConstants.ClaimStatuses, claimStatusList);
            }

            if (claimStatusList != null)
            {
                //if (reportkey == "writeoffreport" || reportkey == "vehicledamagesectionreport")
                //{
                //    lookupValueResult = claimStatusList.Select(x =>
                //       new SelectListItem
                //       {
                //           Text = x.Description,
                //           Value = x.Id.ToString(),

                //           Selected = ((x.Description.Contains("WRITE")) ? true : false),
                //       }).OrderBy(x => x.Text).ToList();
                //}
                //else if (reportkey == "vehiclestobereleasedreport")
                //{
                //    lookupValueResult = claimStatusList.Select(x =>
                //       new SelectListItem
                //       {
                //           Text = x.Description,
                //           Value = x.Id.ToString(),

                //           Selected = ((x.Description.Contains("PENDING")) ? true : false),
                //       }).OrderBy(x => x.Text).ToList();
                //}
                //else if (reportkey == "stolen-recoveredreport")
                //{
                //    lookupValueResult = claimStatusList.Select(x =>
                //      new SelectListItem
                //      {
                //          Text = x.Description,
                //          Value = x.Id.ToString(),

                //          Selected = ((x.Description.Contains("STOLEN")) ? true : false),
                //      }).OrderBy(x => x.Text).ToList();
                //}
                //else
                //{
                    lookupValueResult = claimStatusList.Select(x =>
                      new SelectListItem
                      {
                          Text = x.Description,
                          Value = x.Id.ToString(),
                          Selected=((riskReportDefaultSelectionDto.Any() && riskReportDefaultSelectionDto.Where(y => y.PropId.Equals(x.Id)).Any()) ? true : false)
                      }).OrderBy(x => x.Text).ToList();
                //}
            }

            return lookupValueResult;
        }

        /// <summary>
        /// This method is used to get write off types
        /// </summary>
        /// <returns></returns>
        internal static List<SelectListItem> GetWriteOffTypeListItems()
        {
            List<SelectListItem> lookupValueResult = new List<SelectListItem>();

            IEnumerable<WriteOffTypeDTO> writeOffTypes = Cache.Get<List<WriteOffTypeDTO>>(Constants.CacheConstants.WriteOffTypes);

            if (writeOffTypes == null)
            {
                writeOffTypes = _lookUpService.GetAllWriteOffTypesAsync().Result;
                Cache.Add(Constants.CacheConstants.WriteOffTypes, writeOffTypes);
            }

            if (writeOffTypes != null)
            {
                lookupValueResult = writeOffTypes.Select(x =>
                   new SelectListItem
                   {
                       Text = x.Description,
                       Value = x.Id.ToString(),
                   }).OrderBy(x => x.Text).ToList();
            }
            return lookupValueResult;
        }
    }
}