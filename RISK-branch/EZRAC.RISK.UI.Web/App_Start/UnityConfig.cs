using EZRAC.Core.Caching;
using EZRAC.Core.Caching.ContextCache;
using EZRAC.Core.Data.EntityFramework;
using EZRAC.RISK.EntityFramework;
using EZRAC.Risk.UI.Web.DocumentGenerator;
using EZRAC.Risk.UI.Web.DocumentGenerator.Implementation;
using EZRAC.RISK.Services.Contracts;
using EZRAC.RISK.Services.Implementation;
using EZRAC.RISK.Util.Common;
using Microsoft.Practices.ServiceLocation;
using Microsoft.Practices.Unity;
using System;
using System.Data.Entity;


namespace EZRAC.Risk.UI.Web.App_Start
{
    /// <summary>
    /// Specifies the Unity configuration for the main container.
    /// </summary>
    public class UnityConfig
    {
        #region Unity Container
        private static Lazy<IUnityContainer> container = new Lazy<IUnityContainer>(() =>
        {
            var container = new UnityContainer();
            RegisterTypes(container);
            return container;
        });

        /// <summary>
        /// Gets the configured Unity container.
        /// </summary>
        public static IUnityContainer GetConfiguredContainer()
        {
            return container.Value;
        }
        #endregion

        /// <summary>Registers the type mappings with the Unity container.</summary>
        /// <param name="container">The unity container to configure.</param>
        /// <remarks>There is no need to register concrete types such as controllers or API controllers (unless you want to 
        /// change the defaults), as Unity allows resolving a concrete type even if it was not previously registered.</remarks>
        public static void RegisterTypes(IUnityContainer container)
        {
            // NOTE: To load from web.config uncomment the line below. Make sure to add a Microsoft.Practices.Unity.Configuration to the using statements.
            // container.LoadConfiguration();

            // TODO: Register your types here
            container.RegisterType<DbContext, CRMSContext>(new PerRequestLifetimeManager());
            //container.RegisterType<DbContext, CRMSContext>();
           //container.RegisterType<DbContext, CRMSContext>(new HierarchicalLifetimeManager());
            //container.RegisterType(typeof(IGenericRepository<>), typeof(GenericRepository<>));  
            container.RegisterType(typeof(IGenericRepository<>), typeof(GenericRepository<>), new PerRequestLifetimeManager());            
            container.RegisterType<IUserService, UserService>();
            container.RegisterType<IDriverAndIncService, DriverService>();
            container.RegisterType<IClaimService, ClaimService>();
            container.RegisterType<IBillingService, BillingService>();
            container.RegisterType<IRiskWriteOffService, RiskWriteOffService>();
            container.RegisterType<INotesService, NotesService>();
            container.RegisterType<ILookUpService, LookupService>();
            container.RegisterType<ITSDService, TSDService>();
            container.RegisterType<IRiskFileService, RiskFileService>();
            container.RegisterType<IPaymentService, PaymentService>();
            container.RegisterType<IEmailGeneratorService, EmailGeneratorService>();
            container.RegisterType<ICacheProvider, ContextCache>();
            container.RegisterType<IAdminService, AdminService>();
            container.RegisterType<IVehicleSectionService, VehicleSectionService>();
            container.RegisterType<IUserRoleService, UserRoleService>();
            container.RegisterType<IInsuranceCompanyService, InsuranceService>();
            container.RegisterType<IPoliceAgencyService, PoliceAgencyService>();
            container.RegisterType<ILocationService, LocationService>();
            container.RegisterType<ICompanyService, CompanyService>();
            container.RegisterType<IDocumentGeneratorService, DocumentGeneratorService>();
            container.RegisterType<IDocumentGenerator, Ardl>(DocumentTypes.AR_DL1_RTR.ToString());
            container.RegisterType<IRiskReport, RiskReportService>();
            container.RegisterType<IDocumentsReceivedService, DocumentsReceivedService>();
            container.RegisterType<IRiskReportDefaultSelectionService, RiskReportDefaultSelectionService>();
            container.RegisterType<IDocumentGenerator, IntroductionLetter>(DocumentTypes.Introduction_Letter.ToString());
            container.RegisterType<IDocumentGenerator, JnrDebtorPlacementForm>(DocumentTypes.JNR_Debtor_Placement_Form.ToString());
            container.RegisterType<IDocumentGenerator, AttorneyAcknowledgementLetter>(DocumentTypes.Attorney_Acknowledgement_Letter.ToString());
            container.RegisterType<IDocumentGenerator, CdwNotVoided>(DocumentTypes.CDW_NotVoided_ULL.ToString());
            container.RegisterType<IDocumentGenerator, ClientInfoAndAuthorization>(DocumentTypes.Client_Info_and_Authorization.ToString());
            container.RegisterType<IDocumentGenerator, CustomerFaxCoverSheet>(DocumentTypes.Customer_Fax_Cover_Sheet.ToString());
            container.RegisterType<IDocumentGenerator, NationalCasualtyInsuranceCover>(DocumentTypes.National_Casualty_Insurance_Cover.ToString());
            container.RegisterType<IDocumentGenerator, CustomerServiceInformation>(DocumentTypes.Customer_Service_Information.ToString());
            container.RegisterType<IDocumentGenerator, ThirdPartyDemandLetter>(DocumentTypes.Demand_Letter_1_3rd_Party.ToString());
            container.RegisterType<IDocumentGenerator, PaymentBalanceDue>(DocumentTypes.Payment_Balance_due.ToString());
            container.RegisterType<IDocumentGenerator, PaymentPlan>(DocumentTypes.Payment_Plan.ToString());
            container.RegisterType<IDocumentGenerator, ThirdPartyInsuranceDemandLetter>(DocumentTypes.Demand_Letter_1_3rd_Party_Insurance.ToString());
            container.RegisterType<IEFHelper, EFHelper>();
            container.RegisterType<IDocumentGenerator, PayoffForm>(DocumentTypes.Payoff_Form.ToString());
            container.RegisterType<IDocumentGenerator, RepossesionAutherizationNationwideRepo>(DocumentTypes.Repo_Auth_Nationwide_Repo.ToString());
            container.RegisterType<IDocumentGenerator, RenterDemandLetterOneAndTwo>(DocumentTypes.Demand_Letter_1_Renter.ToString());
            container.RegisterType<IDocumentGenerator, RenterDemandLetterThreeAndFour>(DocumentTypes.Demand_Letter_3_Renter.ToString());
            container.RegisterType<IDocumentGenerator, ReposessionAuthorization>(DocumentTypes.Reposession_Authorization.ToString());
            container.RegisterType<IDocumentGenerator, SalvageBid>(DocumentTypes.Salvage_Bid_Accepted.ToString());
            container.RegisterType<IDocumentGenerator, GflRenter>(DocumentTypes.GFL_Renter.ToString());
            container.RegisterType<IDocumentGenerator, TicketAdminFee>(DocumentTypes.Ticket_Admin_Fee.ToString());
            container.RegisterType<IDocumentGenerator, InsuranceFaxCover>(DocumentTypes.Insurance_Fax_Cover_Sheet.ToString());
            container.RegisterType<IDocumentGenerator, TicketAffidavit>(DocumentTypes.Ticket_Affidavit.ToString());
            container.RegisterType<IDocumentGenerator, VehicleRelease>(DocumentTypes.Vehicle_Release.ToString());
            container.RegisterType<IDocumentGenerator, ViolationAdminFee>(DocumentTypes.Violation_Admin_Fee.ToString());
            container.RegisterType<IDocumentGenerator, WholeSaleBillOfSale>(DocumentTypes.Wholesale_Bill_of_Sale.ToString());
            container.RegisterType<IDocumentGenerator, BodyShopRepair>(DocumentTypes.Body_Shop_Repair.ToString());
            container.RegisterType<IDocumentGenerator, ReportedStolen>(DocumentTypes.Reported_Stolen.ToString());
            container.RegisterType<ITrackingService, TrackingService>();
            UnityServiceLocator locator = new UnityServiceLocator(container);
            ServiceLocator.SetLocatorProvider(() => locator);
        }
    }
}
