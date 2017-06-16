using Microsoft.VisualStudio.TestTools.UnitTesting;
using EZRAC.Core.Data.EntityFramework;
using EZRAC.RISK.Domain;
using EZRAC.Risk.Services.Test.Helpers;
using EZRAC.RISK.Services.Implementation;
using EZRAC.RISK.Services.Contracts;
using System.Threading.Tasks;
using System.Linq;
namespace UnitTestProject.UnitTestServices
{
    [TestClass]
    public class RiskReportDefaultSelectionServiceUnitTest
    {
        #region Private variables
        IGenericRepository<RiskReportDefaultselection> _mockRiskReportDefaultselectionRepository = null;
        IRiskReportDefaultSelectionService _riskReportDefaultSelectionService = null;
        #endregion

        [TestInitialize]
        public void Setup()
        {
            _mockRiskReportDefaultselectionRepository = new MockGenericRepository<RiskReportDefaultselection>
                                            (DomainBuilder.GetList<RiskReportDefaultselection>()).SetUpRepository();

            _riskReportDefaultSelectionService = new RiskReportDefaultSelectionService(_mockRiskReportDefaultselectionRepository);
        }

        [TestMethod]
        public void Test_method_for_get_report_default_section()
        {
            string reportKey = _mockRiskReportDefaultselectionRepository.AsQueryable.Select(x => x.ReportKey).FirstOrDefault();

            string property = _mockRiskReportDefaultselectionRepository.AsQueryable.Select(x => x.Property).FirstOrDefault();

            var riskReportDefaultSelectionDto = _riskReportDefaultSelectionService.GetReportDefaultSelectionAsync(reportKey, property);

            Assert.IsTrue(riskReportDefaultSelectionDto.Count() > 0 ? true : false);
        }
    }
}
