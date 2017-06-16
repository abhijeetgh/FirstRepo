using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using EZRAC.Core.Data.EntityFramework;
using EZRAC.RISK.Domain;
using EZRAC.Risk.Services.Test.Helpers;
using EZRAC.RISK.Services.Implementation;
using System.Threading.Tasks;
using EZRAC.RISK.Services.Contracts.Dtos;
using System.Linq;
using System.Data.Entity;
using System.Collections.Generic;
using EZRAC.RISK.Util.Common;
using EZRAC.RISK.Services.Contracts;
using FizzWare.NBuilder;

namespace UnitTestProject.UnitTestServices
{
    [TestClass]
    public class VehicleSectionServiceUnitTest
    {

        #region Private variables
        IGenericRepository<RiskDamageType> _mockDamageTypeRepository = null;
        IGenericRepository<RiskDamage> _mockDamageRepository = null;

        IVehicleSectionService _vehicleSectionService = null;

        #endregion
        [TestInitialize]
        public void Setup()
        {
            _mockDamageTypeRepository = new MockGenericRepository<RiskDamageType>(DomainBuilder.GetRiskDamageTypes()).SetUpRepository();
            _mockDamageRepository = new MockGenericRepository<RiskDamage>(DomainBuilder.GetRiskDamages()).SetUpRepository();

            _vehicleSectionService = new VehicleSectionService(_mockDamageTypeRepository, _mockDamageRepository);
        }


        [TestMethod]
        public void Test_method_for_get_vehicle_section()
        {
            long id = _mockDamageTypeRepository.AsQueryable.Select(x => x.Id).FirstOrDefault();

            var vehiclesection =  _vehicleSectionService.GetVehicleSection(id).Result;

            Assert.IsNotNull(vehiclesection);
        }

        public void Test_method_for_validating_a_vehicle_section ()
        {
            long id = _mockDamageTypeRepository.AsQueryable.Select(x => x.Id).Max()+1;

            string section = DomainBuilder.GetRandomString();

            long userId = _mockDamageTypeRepository.AsQueryable.Select(x => x.UpdatedBy).Max();

            bool success = _vehicleSectionService.AddOrUpdateVehicleSection(id, section, userId).Result;

            Assert.IsFalse(success);
        }

        [TestMethod]
        public void Test_method_for_update_a_vehicle_section()
        {
            long id = _mockDamageTypeRepository.AsQueryable.Select(x => x.Id).Max();

            string section = DomainBuilder.GetRandomString();

            long userId = _mockDamageTypeRepository.AsQueryable.Select(x => x.UpdatedBy).Max();

           bool vehicle =   _vehicleSectionService.AddOrUpdateVehicleSection(id, section, userId).Result;

           Assert.IsTrue(vehicle);
        }

        [TestMethod]
        public void Test_method_for_add_a_vehicle_section()
        {
            string section = DomainBuilder.GetRandomString();

            long userId = _mockDamageTypeRepository.AsQueryable.Select(x => x.UpdatedBy).Max();

            //For Add Vehicle Section Id Must Be 0;

            bool vehicleSection =  _vehicleSectionService.AddOrUpdateVehicleSection(0, section, userId).Result;

            Assert.IsTrue(vehicleSection);
        }

        [TestMethod]
        public void Test_method_for_verify_already_exist_vehicle_section()
        {
            RiskDamage riskDamage = _mockDamageRepository.AsQueryable.FirstOrDefault();

            bool exists = _vehicleSectionService.IsVehicleSectionAlreadyUsed(riskDamage.DamageTypeId);

            Assert.IsTrue(exists);
        }

        [TestMethod]
        public void Test_method_for_delete_a_used_vehicle_section()
        {
            long damageTypeId = _mockDamageRepository.AsQueryable.Select(x => x.DamageTypeId).Max();

            var delvehiclesection = _vehicleSectionService.DeleteVehicleSection(damageTypeId).Result;

            Assert.IsFalse(delvehiclesection);
        }

        [TestMethod]
        public void Test_method_for_delete_a_unused_vehicle_section()
        {
            var damageTypeId = _mockDamageRepository.AsQueryable.Select(x => x.DamageTypeId).Max() + 1;

            var damageType = _mockDamageTypeRepository.AsQueryable.FirstOrDefault();

            damageType.Id = damageTypeId;

            bool delvehiclesection = _vehicleSectionService.DeleteVehicleSection(damageTypeId).Result;

            Assert.IsTrue(delvehiclesection);
        }

      
        
    }
}
