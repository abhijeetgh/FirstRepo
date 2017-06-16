using EZRAC.Core.Data.EntityFramework;
using EZRAC.Risk.Services.Test.Helpers;
using EZRAC.RISK.Domain;
using EZRAC.RISK.Services.Contracts;
using EZRAC.RISK.Services.Contracts.Dtos;
using EZRAC.RISK.Services.Implementation;
using FizzWare.NBuilder;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EZRAC.Risk.Services.Test.UnitTestServices
{
    [TestClass]
   public class PoliceAgencyServiceUnitTest
   {
       #region Private Variables

       IGenericRepository<RiskPoliceAgency> _mockRiskPoliceAgencyRepository = null;
       IGenericRepository<RiskIncident> _mockIncidentRepository = null;
       IPoliceAgencyService _mockPoliceAgencyService = null;

       #endregion

      [TestInitialize]
      public void Setup()
      {
           _mockRiskPoliceAgencyRepository = new MockGenericRepository<RiskPoliceAgency>(DomainBuilder.GetRiskPoliceAgencies()).SetUpRepository();
           _mockIncidentRepository = new MockGenericRepository<RiskIncident>(DomainBuilder.GetRiskIncidents()).SetUpRepository();
           _mockPoliceAgencyService = new PoliceAgencyService(_mockRiskPoliceAgencyRepository, _mockIncidentRepository);
      }

      [TestMethod]
      public void Test_method_for_get_police_agency_detail_by_id()
      {
           long id = Convert.ToInt64(_mockRiskPoliceAgencyRepository.AsQueryable.Select(p => p.Id).Max());
           var policeAgencyDto = _mockPoliceAgencyService.GetPoliceAgencyDetailById(id).Result;
           Assert.IsNotNull(policeAgencyDto);
      }

      [TestMethod]
      public void Test_method_for_add_police_agency()
      {
          var policeAgencyDto = DtoBuilder.Get<PoliceAgencyDto>();
          policeAgencyDto.Id = 0;
          policeAgencyDto.AgencyName = Builder<string>.CreateNew().ToString();
          Assert.IsTrue(_mockPoliceAgencyService.AddOrUpdatePoliceAgency(policeAgencyDto, 2).Result);
      }
      [TestMethod]
      public void Test_method_for_update_police_agency()
      {
          var policeAgencyDto = DtoBuilder.Get<PoliceAgencyDto>();
          policeAgencyDto.Id = _mockRiskPoliceAgencyRepository.GetAllAsync().Result.FirstOrDefault().Id;
          Assert.IsNotNull(_mockPoliceAgencyService.AddOrUpdatePoliceAgency(policeAgencyDto, 1).Result);
      }
      [TestMethod]
      public void Test_method_for_duplicate_police_agency()
      {
          var policeAgencyDto = DtoBuilder.Get<PoliceAgencyDto>();
          policeAgencyDto.Id = 0;
          //policeAgencyDto.Id = _mockRiskPoliceAgencyRepository.AsQueryable.Select(p => p.Id).FirstOrDefault();
          // policeAgencyDto.AgencyName = "Agency2";
          Assert.IsFalse( _mockPoliceAgencyService.AddOrUpdatePoliceAgency(policeAgencyDto, 1).Result);
      }
      [TestMethod]
      public void Test_method_for_check_is_police_agency_already_used()
      {
          long policeAgencyId = _mockRiskPoliceAgencyRepository.AsQueryable.Select(p => p.Id).FirstOrDefault();
          var policeAgencydto = DtoBuilder.Get<PoliceAgencyDto>();
          policeAgencydto.Id = Convert.ToInt16(policeAgencyId);
          Assert.IsTrue(_mockPoliceAgencyService.IsPoliceAgencyAlreadyUsed(policeAgencyId));
          Assert.IsFalse( _mockPoliceAgencyService.DeletePoliceAgency(policeAgencydto, 1).Result);


      }
      [TestMethod]
      public void Test_method_for_delete_police_agency()
      {
          long policeAgencyId = _mockRiskPoliceAgencyRepository.AsQueryable.Select(p => p.Id).Max();
          long incidentPoliceAgencyId = Convert.ToInt64(_mockIncidentRepository.AsQueryable.Select(i => i.PoliceAgencyId).FirstOrDefault());

          var policeAgencydto = DtoBuilder.Get<PoliceAgencyDto>();
          policeAgencydto.Id = Convert.ToInt32(policeAgencyId);
          var policeAgency =  _mockPoliceAgencyService.DeletePoliceAgency(policeAgencydto, 1).Result;
          Assert.IsTrue(policeAgency);

      }
    

        
   }
}
