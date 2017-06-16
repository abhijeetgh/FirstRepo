using EZRAC.RISK.Domain;
using FizzWare.NBuilder;
using Ploeh.AutoFixture;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNet.Identity;

namespace EZRAC.Risk.Services.Test.Helpers
{
    public static class DomainBuilder
    {
        public static List<Company> GetCompanies()
        {
            return Builder<Company>.CreateListOfSize(5).All()
                                    .With(x => x.Locations = GetLocations())
                                    .With(x => x.IsDeleted = false)
                                    .Build().ToList();          
        }

        public static List<Location> GetLocations()
        {

            return Builder<Location>.CreateListOfSize(5).All()
                                .With(x=> x.Company = Get<Company>())
                                .With(x => x.Incidents = GetList<RiskIncident>())
                                .With(x => x.Sli = GetSlies())
                                .With(x => x.OpenLocationClaims = GetClaims())
                                .With(x => x.CloseLocationClaims = GetClaims())
                                .With(x=>x.Users = GetList<User>())
                                .With(x => x.IsDeleted = false)
                                .Build().ToList();
        }

        public static List<Claim> GetClaims()
        {

            return Builder<Claim>.CreateListOfSize(5).All()
                                .With(x => x.RiskVehicle = Get<RiskVehicle>())
                                .With(x => x.RiskClaimStatus = Get<RiskClaimStatus>())
                                .With(x => x.RiskLossType = Get<RiskLossType>())
                                .With(x => x.OpenLocation = Get<Location>())
                                .With(x => x.CloseLocation = Get<Location>())
                                .With(x => x.RiskContract = Get<RiskContract>())
                                .With(x => x.RiskNonContract = Get<RiskNonContract>())
                                .With(x => x.RiskIncident = Get<RiskIncident>())
                                .With(x => x.RiskDrivers = GetRiskDrivers())
                                .With(x => x.RiskClaimApprovals = GetRiskClaimApprovals())
                                .With(x => x.RiskNotes = GetList<RiskNote>())
                                .With(x => x.RiskDamages = GetRiskDamages())
                                .With(x => x.RiskBillings = GetRiskBillings())
                                .With(x => x.RiskPayments = GetRiskPayments())
                                .With(x => x.AssignedUser = Get<User>())
                                .With(x => x.RiskDocumentsReceived = Get<RiskDocumentsReceived>())
                                .With(x => x.ClaimTrackings = GetList<ClaimTrackings>())
                                .Build().ToList();
        }

        public static List<RiskIncident> GetRiskIncidents()
        {

            return Builder<RiskIncident>.CreateListOfSize(5).All()
                                .With(x => x.Claim = Get<Claim>())
                                .With(x => x.Location = Get<Location>())
                                .With(x => x.PoliceAgency = Get<RiskPoliceAgency>())
                                .Build().ToList();
        }

        public static List<RiskClaimStatus> GetRiskClaimStatusses()
        {

            return Builder<RiskClaimStatus>.CreateListOfSize(5).All()
                                .With(x => x.Claims = GetClaims())
                                .Build().ToList();
        }

        public static List<RiskLossType> GetRiskLossTypes()
        {

            return Builder<RiskLossType>.CreateListOfSize(5).All()
                                .With(x => x.Claims = GetClaims())
                                .Build().ToList();
        }

        public static List<RiskBilling> GetRiskBillings()
        {

            return Builder<RiskBilling>.CreateListOfSize(5).All()
                                .With(x => x.Claim = Get<Claim>())
                                .With(x => x.RiskBillingType = Get<RiskBillingType>())
                                .Build().ToList();
        }

        public static List<RiskPayment> GetRiskPayments()
        {

            return Builder<RiskPayment>.CreateListOfSize(5).All()
                                .With(x => x.Claim = Get<Claim>())
                                .With(x => x.RiskPaymentType = Get<RiskPaymentType>())
                                .Build().ToList();
        }

        public static List<RiskDocumentType> GetRiskDocumentTypes()
        {

            return Builder<RiskDocumentType>.CreateListOfSize(5).All()
                                .With(x => x.RiskDocumentCategory = Get<RiskDocumentCategory>())
                                .Build().ToList();
        }

        public static List<RiskDocumentCategory> GetRiskDocumentCategories()
        {

            return Builder<RiskDocumentCategory>.CreateListOfSize(5).All()
                                .With(x => x.RiskDocumentTypes = GetRiskDocumentTypes())
                                .Build().ToList();
        }

        public static List<RiskDriver> GetRiskDrivers()
        {
            Random random = new Random();

            return Builder<RiskDriver>.CreateListOfSize(5).All()
                                .With(x => x.Claim =  Get<Claim>())
                                .With(x=>x.ClaimId = random.Next(1,3))
                                .With(x=>x.RiskDriverInsurance = Get<RiskDriverInsurance>())
                                .With(x => x.RiskDriverInsurance.RiskInsurance = Get<RiskInsurance>())
                                .Build().ToList();
        }

        public static List<RiskVehicle> GetRiskVehicles()
        {

            return Builder<RiskVehicle>.CreateListOfSize(5).All()
                                .With(x => x.Claims = GetClaims())
                                .With(x => x.RiskDamages = GetRiskDamages())
                                .Build().ToList();
        }

        public static List<Sli> GetSlies()
        {

            return Builder<Sli>.CreateListOfSize(5).All()
                                .With(x => x.SliLocations = GetList<Location>())
                                .Build().ToList();
        }

        public static List<User> GetUsers()
        {
            PasswordHasher passwordHasher = new PasswordHasher();

            return Builder<User>.CreateListOfSize(5).All()
                                .With(x => x.Claims = GetList<Claim>())
                                .With(x => x.PasswordHash = passwordHasher.HashPassword(x.PasswordHash))
                                .With(x => x.Locations = GetList<Location>())
                                .With(x => x.ClaimTrackings = GetList<ClaimTrackings>())
                                .With(x => x.IsActive = true)
                                .With(x => x.IsDeleted = false)
                                .Build().ToList();
        }

        public static List<RiskContract> GetRiskContracts()
        {

            return Builder<RiskContract>.CreateListOfSize(5).All()
                                .With(x => x.Claims = GetList<Claim>())
                                .Build().ToList();
        }

        public static List<RiskNonContract> GetRiskNonContracts()
        {

            return Builder<RiskNonContract>.CreateListOfSize(5).All()
                                .With(x => x.Claims = GetList<Claim>())
                                .Build().ToList();
        }

        public static List<RiskClaimApproval> GetRiskClaimApprovals()
        {

            return Builder<RiskClaimApproval>.CreateListOfSize(5).All()
                                .With(x => x.ClaimStatus = Get<RiskClaimStatus>())
                                .Build().ToList();
        }

        public static List<RiskPoliceAgency> GetRiskPoliceAgencies()
        {

            return Builder<RiskPoliceAgency>.CreateListOfSize(6).All()
                                .With(x => x.Incidents = GetList<RiskIncident>())
                                .Build().ToList();
        }

        public static List<RiskDamage> GetRiskDamages()
        {

            return Builder<RiskDamage>.CreateListOfSize(5).All()
                                .With(x => x.RiskDamageType = Get<RiskDamageType>())
                                .With(x => x.RiskVehicle = Get<RiskVehicle>())
                                .Build().ToList();
        }

        public static List<RiskDriverInsurance> GetRiskDriverInsurances()
        {

            return Builder<RiskDriverInsurance>.CreateListOfSize(5).All()
                                .With(x => x.RiskDriver = Get<RiskDriver>())
                                .With(x => x.RiskInsurance = Get<RiskInsurance>())
                                .Build().ToList();
        }

        public static List<RiskInsurance> GetRiskInsurances()
        {

            return Builder<RiskInsurance>.CreateListOfSize(5).All()
                                .With(x => x.RiskDriverInsurances = GetRiskDriverInsurances())
                                .Build().ToList();
        }
        public static List<RiskFile> GetRiskFiles()
        {
            return Builder<RiskFile>.CreateListOfSize(6).All()
                .With(x => x.Claim = Get<Claim>())
                .With(x => x.FileType = Get<RiskFileTypes>())
                .With(x => x.Claim.ClaimTrackings = GetList<ClaimTrackings>())
                .Build().ToList();
        }

        public static List<RiskNote> GetRiskNotes()
        {

            return Builder<RiskNote>.CreateListOfSize(5).All().Build().ToList();
        }

        public static List<RiskNoteTypes> GetRiskNotesTypes()
        {
            return Builder<RiskNoteTypes>.CreateListOfSize(5).All().Build().ToList();
        }

        public static List<RiskDamageType> GetRiskDamageTypes()
        {
            return Builder<RiskDamageType>.CreateListOfSize(5).All()
                                          .With(x => x.RiskDamage).Build().ToList();
            
        }

        public static List<RiskFileTypes> GetRiskFileTypes()
        {
            return Builder<RiskFileTypes>.CreateListOfSize(5).All()
                                          .With(x => x.RiskFiles).Build().ToList();

        }

        public static List<RiskBillingType> GetRiskBillingTypes()
        {
            return Builder<RiskBillingType>.CreateListOfSize(5).All()
                                          .With(x => x.RiskBillings).Build().ToList();

        }

        public static List<RiskPaymentType> GetRiskPaymentTypes()
        {
            return Builder<RiskPaymentType>.CreateListOfSize(5).All().Build().ToList();

        }

        public static List<RiskTrackings> GetRiskTrackings()
        {
            return Builder<RiskTrackings>.CreateListOfSize(5).All()
                                          .With(x => x.Next = GetList<RiskTrackings>())
                                          .With(x => x.Previous = GetList<RiskTrackings>())
                                          .With(x => x.RiskTrackingTypes = Get<RiskTrackingTypes>())
                                          .With(x => x.ClaimTrackings = GetList<ClaimTrackings>())
                                         .Build().ToList();

        }

        public static List<ClaimTrackings> GetClaimTrackings()
        {
            return Builder<ClaimTrackings>.CreateListOfSize(5).All()
                                          .With(x => x.RiskTrackings = Get<RiskTrackings>())
                                          .With(x => x.Claim = Get<Claim>())
                                          .With(x => x.User = Get<User>())
                                         .Build().ToList();

        }

        public static List<Permission> GetPermissions()
        {
            return Builder<Permission>.CreateListOfSize(5).All()
                                          .With(x => x.UserRoles = GetList<UserRole>())
                                          .With(x => x.PermissionLevel = GetList<PermissionLevel>())
                                          .With(x => x.RiskCategory = GetList<RiskCategory>())
                                         .Build().ToList();

        }

        public static List<UserRole> GetUserRoleInfo()
        {
            return Builder<UserRole>.CreateListOfSize(5).All()
                                      .With(x => x.Users = GetList<User>())
                                      .With(x => x.Permissions = GetList<Permission>()).Build().ToList();
        }

        public static List<RiskReport> GetRiskReports()
        {
            return Builder<RiskReport>.CreateListOfSize(5).All().With(x => x.RiskReportCategory = Get<RiskReportCategory>()).Build().ToList();
        }

        public static List<RiskReportCategory> GetRiskReportCategory()
        {
            return Builder<RiskReportCategory>.CreateListOfSize(5).All().With(x => x.RiskReport = GetList<RiskReport>()).Build().ToList();
        }

        public static List<TEntity> GetList<TEntity>() where TEntity : class
        {
            return Builder<TEntity>.CreateListOfSize(5).All().Build().ToList();
        }

            
        public static TEntity Get<TEntity>() where TEntity : class
        {
            return Builder<TEntity>.CreateNew().Build();
        }

        public static string GetRandomString ()
        {
            var generator = new RandomGenerator();

            return generator.Phrase(15);
        }

        public static List<RiskWriteOffType> GetRiskWriteOffTypes()
        {
            return Builder<RiskWriteOffType>.CreateListOfSize(5).All().Build().ToList();
        }

        public static List<RiskWriteOff> GetRiskWriteOffs()
        {
            return Builder<RiskWriteOff>.CreateListOfSize(5).All().Build().ToList();
        }
    }
}
