using EZRAC.Core.Data.EntityFramework;
using EZRAC.RISK.Domain;
using EZRAC.RISK.EntityFramework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.Entity;
using Microsoft.AspNet.Identity;
using EZRAC.RISK.Services.Contracts.Dtos;
using EZRAC.RISK.Services.Implementation;
using EZRAC.RISK.Services.Contracts;
using EZRAC.RISK.Services.Implementation.Helper;
using Microsoft.Practices.Unity;
using Microsoft.Practices.ServiceLocation;



namespace TestClient
{
    class Program
    {

        private static Lazy<IUnityContainer> container = new Lazy<IUnityContainer>(() =>
        {
            var container = new UnityContainer();
            RegisterTypes(container);
            return container;
        });


        /// <summary>Registers the type mappings with the Unity container.</summary>
        /// <param name="container">The unity container to configure.</param>
        /// <remarks>There is no need to register concrete types such as controllers or API controllers (unless you want to 
        /// change the defaults), as Unity allows resolving a concrete type even if it was not previously registered.</remarks>
        public static void RegisterTypes(IUnityContainer container)
        {
            // NOTE: To load from web.config uncomment the line below. Make sure to add a Microsoft.Practices.Unity.Configuration to the using statements.
            // container.LoadConfiguration();

            // TODO: Register your types here
            container.RegisterType<DbContext, CRMSContext>();
            //container.RegisterType<DbContext, CRMSContext>();
            //container.RegisterType<DbContext, CRMSContext>(new HierarchicalLifetimeManager());
            //container.RegisterType(typeof(IGenericRepository<>), typeof(GenericRepository<>));  
            container.RegisterType(typeof(IGenericRepository<>), typeof(GenericRepository<>));
            container.RegisterType<IUserService, UserService>();
            container.RegisterType<IDriverAndIncService, DriverService>();
            container.RegisterType<IClaimService, ClaimService>();
            container.RegisterType<IBillingService, BillingService>();
            container.RegisterType<INotesService, NotesService>();
            container.RegisterType<ILookUpService, LookupService>();
            container.RegisterType<ITSDService, TSDService>();
            container.RegisterType<IRiskFileService, RiskFileService>();
            container.RegisterType<IPaymentService, PaymentService>();
            container.RegisterType<IEmailGeneratorService, EmailGeneratorService>();
            
            container.RegisterType<IAdminService, AdminService>();
            container.RegisterType<IVehicleSectionService, VehicleSectionService>();
            container.RegisterType<IUserRoleService, UserRoleService>();
            container.RegisterType<IInsuranceCompanyService, InsuranceService>();
            container.RegisterType<IPoliceAgencyService, PoliceAgencyService>();
            container.RegisterType<ILocationService, LocationService>();
            container.RegisterType<ICompanyService, CompanyService>();
            container.RegisterType<IDocumentGeneratorService, DocumentGeneratorService>();
           
            container.RegisterType<IRiskReport, RiskReportService>();
            container.RegisterType<IDocumentsReceivedService, DocumentsReceivedService>();
            container.RegisterType<IRiskReportDefaultSelectionService, RiskReportDefaultSelectionService>();
           
            container.RegisterType<ITrackingService, TrackingService>();
            UnityServiceLocator locator = new UnityServiceLocator(container);
            ServiceLocator.SetLocatorProvider(() => locator);
        }

        static void Main(string[] args)
        {
            var dbContext = new CRMSContext();

           // var _tsdContext = new TSDContext();

            //var _tsdService = container.Value.Resolve<ITSDService>();

            //_tsdService.GetFullContractInfoFromTSD("ORD-26138", "019982091577");

            //Get Risk LossType from DB
            //BaseRepository<RiskLossType> lossTypeRepository = new BaseRepository<RiskLossType>(dbContext);
            //var lossTypes = lossTypeRepository.GetAll().ToList();

            //GenericRepository<RiskIncident> incidentRepository = new GenericRepository<RiskIncident>(dbContext);

            //GenericRepository<Location> locationRepository = new GenericRepository<Location>(dbContext);

           // AddBilling(dbContext);

            //ClaimDriverDelete(dbContext);

            //var lossTypes = lossTypeRepository.GetAll().ToList();

            //BaseRepository<RiskClaimStatus> riskClaimStatusRepository = new BaseRepository<RiskClaimStatus>(dbContext);
            //var riskClaimStatusList = riskClaimStatusRepository.GetAll().ToList();

            //Get Claims List from DB

            //BaseService<Location> claimService = new BaseService<Location>(dbContext);

            //var claim = claimRepository.GetByIdAsync(3);
            //claimRepository.DeleteAsync(claim.Result);

            //var claims = claimRepository.AsQueryable.IncludeMultiple(x => x.RiskClaimStatus, x => x.RiskLossType).ToListAsync().Result;
            //var claims1 = claimRepository.GetAllAsync().Result;

            //Task t = asyncMethod();
            //t.ContinueWith((str) =>
            //{
            //    Console.WriteLine(str.Status.ToString());
            //    Console.WriteLine("Main end");
            //});
            //t.Wait();
            //GenericRepository<Role> roleRepository = new GenericRepository<Role>(dbContext);
            //var roles = roleRepository.GetAllAsync().Result;
            //Role role = new Role();
            //role.Name = "Adminstrator";
            //Role role1 = new Role();
            //role.Name = "RiskAgent";
            //Role role2 = new Role();
            //role.Name = "RiskManager";
            //Task.Run(async () =>
            //{
            //    asyncMethod();
            //}).Wait();

            //Console.ReadLine();

            //roleRepository.InsertAsync(role1);
            //roleRepository.InsertAsync(role2);

            //var list = claimService.GetAll();  

            //CRMSContext dbContext = new CRMSContext();

            //CreateClaimMethod(dbContext);

            //ClaimDriverMapping(dbContext);

            //LookUpService lookup=new LookUpService(
            //new GenericRepository<Location>(), 
            //new GenericRepository<User>(),
            //new GenericRepository<RiskLossType>(),
            //new GenericRepository<RiskClaimStatus>(),
            //new GenericRepository<Claim>());


            //TSDService TSDservice = new TSDService();

            //ClaimService claimService = new ClaimService(new GenericRepository<Claim>(),
            //                new GenericRepository<Location>(),
            //                new GenericRepository<User>(),
            //                new GenericRepository<RiskContract>(),
            //                new GenericRepository<RiskVehicle>(),
            //                new GenericRepository<RiskClaimApproval>(),
            //                TSDservice,
            //                lookup);

            //TSDservice.GetFullContractInfoFromTSD("1008t");


            //var result = TSDservice.GetFullContractInfoFromTSDAsync("ATL-298428").Result;

            //Task<ContractInfoFromTSD> t = service.GetFullContractInfoFromTSDAsync("1008t");

            //FullContractDetailsFromTSD fetchedDetailsDTO = t.

            //ClaimDto claim = claimService.UpdateClaimInfoByClaimIdAsync(new ClaimDto() { CloseDate = DateTime.Now.AddDays(5), SelectedOpenLocationId=3,
            //                                                                             SelectedCloseLocationId = 3,
            //                                                                             LabourHour = 15,
            //                                                                             SelectedLossTypeId=1,
            //                                                                             Id=55
            //}).Result;

         //   var location = locationRepository.GetAllAsync().Result;

          //  var result = incidentRepository.AsQueryable.IncludeMultiple(x => x.Location, x => x.PoliceAgency).Where(x => x.Id == 55).FirstOrDefault();
           

            Console.ReadLine();

        }

        private static void CreateClaimMethod(CRMSContext dbContext)
        {
            dbContext.Database.Log = Console.Write;

            GenericRepository<Claim> claimRepository = new GenericRepository<Claim>(dbContext);

            //GenericRepository<User> userRepository = new GenericRepository<User>(dbContext);

            //GenericRepository<RiskDriver> driverRepository = new GenericRepository<RiskDriver>(dbContext);

            //GenericRepository<RiskContract> contractRepository = new GenericRepository<RiskContract>(dbContext);

            GenericRepository<Location> locationRepository = new GenericRepository<Location>(dbContext);

            //var user = userRepository.GetByIdAsync(2).Result;

            //RiskContract contractToAdd = new RiskContract { CDW = true, ContractNumber = "1008t", DailyRate = 12, DaysOut = 5, LDW = true, Miles = 2.5 };

            //List<RiskDriver> drivers = new List<RiskDriver>();

            //drivers.Add(new RiskDriver { Address = "Pune", Address2 = "Pune", DriverTypeId = 1, FirstName = "Ajay", LastName = "Bommena" });

            //drivers.Add(new RiskDriver
            //{
            //    Address = "Pune",
            //    Address2 = "Pune",
            //    DriverTypeId = 2,
            //    FirstName = "AddDriver",
            //    LastName = "Bommena",
            //    RiskInsurance = new RiskInsurance { CompanyName = "ICICI", Deductible = 22.5, Email = "abc@gmail.com" }
            //});


            //var driver = driverRepository.InsertAsync(new RiskDriver
            //{
            //    Address = "Pune",
            //    Address2 = "Pune",
            //    DriverTypeId = 2,
            //    FirstName = "AddDriver",
            //    LastName = "Bommena",
            //    RiskInsurance = new RiskInsurance { CompanyName = "ICICI", Deductible = 22.5, Email = "icici@gmail.com" }
            //}).Result;



            //var claimToAdd = new Claim
            //{
            //    AssignedTo = 2,
            //    AssignedToName = userRepository.GetByIdAsync(2).Result.FirstName,
            //    ClaimStatusId = 1,
            //    RiskContract = contractToAdd,
            //    DateofLoss = DateTime.Now.AddDays(-1),
            //    FollowUpDate = DateTime.Now.AddDays(2),
            //    LabourHour = 12.5,
            //    OpenLocationId = 3,
            //    LossTypeId = 1,
            //    OpenDate = DateTime.Now,
            //    RiskDrivers = drivers,
            //    RiskVehicle = new RiskVehicle { Color = "Red", Location = "Pune", Make = "2014", TagNumber = "AP15AP8080", UnitNumber = "ABC123" },
            //    IsDeleted = false,

            //};

            ////var user = userRepository.GetAllAsync().Result;
            //try
            //{
            //    Claim claim = claimRepository.GetByIdAsync(14).Result;

            //    //claimRepository.AsQueryable.Where(p=>p.Id==14).FirstOrDefault().RiskDrivers.Where(p=>p.)

            //    //var claim = claimRepository.UpdateAsync(claimToAdd);
            //}
            //catch (Exception e)
            //{
            //    Console.WriteLine(e);
            //    throw;
            //}



           // var claims = locationRepository.GetAllAsync().Result;


            //Console.ReadLine();

            //var claimsWithAll = claimRepository.AsQueryable.IncludeMultiple(x => x.OpenLocation,
            //                                                                x=>x.CloseLocation).ToListAsync().Result;

            //claimsWithAll.RiskDrivers[0].RiskInsurance = new RiskInsurance { CompanyName = "ICICI", Deductible = 22.5, Email = "icici@gmail.com" };

           // var claim = claimRepository.UpdateAsync(claimsWithAll);
            
            //var contracts = contractRepository.GetAllAsync().Result;

            //var contractswithClaims = contractRepository.AsQueryable.Include(x => x.Claims).ToList();

           // var drivers = driverRepository.AsQueryable.Include(x => x.RiskInsurance).ToList();
        }


        //private static void ClaimDriverMapping(CRMSContext dbContext)
        //{
        //    dbContext.Database.Log = Console.Write;

        //    GenericRepository<Claim> claimRepository = new GenericRepository<Claim>(dbContext);

        //    GenericRepository<User> userRepository = new GenericRepository<User>(dbContext);

        //    Claim claim = claimRepository.AsQueryable.IncludeMultiple(x => x.ClaimDriverLink.Select(y => y.Driver),
        //                                                              x => x.RiskContract).Where(x => x.Id == 20077).FirstOrDefault();




        //    List<ClaimDriverMap> ClaimDriverLinks = new List<ClaimDriverMap>();

        //    ClaimDriverLinks.Add(new ClaimDriverMap() { Driver = new RiskDriver { Address = "Pune", Address2 = "Pune", DriverTypeId = 1, FirstName = "Ajay", LastName = "Bommena" } });



        //    var driver = new RiskDriver
        //    {
        //        Address = "Pune",
        //        Address2 = "Pune",
        //        DriverTypeId = 2,
        //        FirstName = "AddDriver",
        //        LastName = "Bommena",
        //        RiskDriverInsurance = new RiskDriverInsurance { InsuranceId = 1, InsuranceCompanyName = "ICICI", Deductible = 22, CreditCardPolicyNumber = "1245" }
        //    };

        //    ClaimDriverLinks.Add(new ClaimDriverMap { Driver = driver, DriverId = 1 });


        //    drivers.Add(new RiskDriver
        //    {
        //        Address = "Pune",
        //        Address2 = "Pune",
        //        DriverTypeId = 2,
        //        FirstName = "AddDriver",
        //        LastName = "Bommena",
        //        RiskDriverInsurance = new RiskDriverInsurance { InsuranceId = 1, InsuranceCompanyName = "ICICI", Deductible = 22, CreditCardPolicyNumber = "1245" }
        //    });


        //    var claimToAdd = new Claim
        //    {
        //        AssignedTo = 2,
        //        AssignedToName = userRepository.GetByIdAsync(2).Result.FirstName,
        //        ClaimStatusId = 1,
        //        RiskContract = contractToAdd,
        //        DateofLoss = DateTime.Now.AddDays(-1),
        //        FollowUpDate = DateTime.Now.AddDays(2),
        //        LabourHour = 12.5,
        //        OpenLocationId = 3,
        //        LossTypeId = 1,
        //        OpenDate = DateTime.Now,
        //        ClaimDriverLink = ClaimDriverLinks,
        //        RiskVehicle = new RiskVehicle { Color = "Red", Location = "Pune", Make = "2014", TagNumber = "AP15AP8080", UnitNumber = "ABC123" },
        //        IsDeleted = false,

        //    };

        //  Claim claim2 = claimRepository.InsertAsync(claimToAdd).Result;

        //    Console.ReadLine();

        //}
        public async static Task<string> ClaimDriverDelete(CRMSContext dbContext)
        {


            GenericRepository<Claim> claimRepository = new GenericRepository<Claim>(dbContext);

            dbContext.Database.Log = Console.Write;

            var claim = claimRepository.AsQueryable.Include(c=>c.RiskDrivers).Where(c => c.Id == 20102).FirstOrDefault();

            var drivers = claim.RiskDrivers.ToList();

            foreach (var item in drivers)
            {
                claim.RiskDrivers.Remove(item);
            }

            try
            {
                await claimRepository.UpdateAsync(claim);
            }
            catch (Exception e)
            {
                
                throw;
            }
             

            return "done";
        
        }

        private static void AddBilling(CRMSContext dbContext) {

            GenericRepository<RiskBilling> billingRepository = new GenericRepository<RiskBilling>(dbContext);

            GenericRepository<Claim> claimRepository = new GenericRepository<Claim>(dbContext);

            var claim = claimRepository.AsQueryable.Include(x => x.RiskBillings).Where(c => c.Id == 55).FirstOrDefault();

            claim.RiskBillings.Add(new RiskBilling() { ClaimId = 55, BillingTypeId = 2, Amount = 99,Discount=5 });

            //.UpdateAsync(claim);

            billingRepository.InsertAsync(new RiskBilling() { ClaimId = 55, BillingTypeId = 1, Amount = 125, Discount = 5 });
        
        } 

        public async static Task<string> asyncMethod()
        {
            PasswordHasher passwordHasher = new PasswordHasher();
            var dbContext = new CRMSContext();
            GenericRepository<User> userRepository = new GenericRepository<User>(dbContext);
            User userObj = new User();
            userObj.UserName = "sandip_RA";
            userObj.Email = "sandipi@cybage.com";
            userObj.FirstName = "sandip_RA";
            userObj.LastName = "Ingle";
            userObj.UserRoleID = 2;
            userObj.PasswordHash = passwordHasher.HashPassword("cybage");
            await userRepository.InsertAsync(userObj);

            return "finished";
        }

        public static void SendEmail(){
        
        
        }



    }
}
