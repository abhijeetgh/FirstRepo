using EZRAC.RISK.Domain;
using EZRAC.RISK.EntityFramework.Mapping;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.ModelConfiguration.Conventions;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EZRAC.RISK.EntityFramework
{
    public class CRMSContext : DbContext
    {
        #region Fields        
        #endregion

        #region Constructor(s)

        
        public CRMSContext()
            : base("name=CRMSContext")
        {
            this.Configuration.LazyLoadingEnabled = true;
            this.Configuration.ProxyCreationEnabled = false;
        }       

        #endregion

        #region Properties


        #endregion
        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {

            modelBuilder.Configurations.Add(new UserMap());
            modelBuilder.Configurations.Add(new UserRoleMap());
            modelBuilder.Configurations.Add(new PermissionMapping());
            modelBuilder.Configurations.Add(new RiskLossTypeMapping());
            modelBuilder.Configurations.Add(new RiskClaimStatusMapping());
            modelBuilder.Configurations.Add(new ClaimMapping());
            modelBuilder.Configurations.Add(new LocationMapping());
            modelBuilder.Configurations.Add(new DriverMapping());
            modelBuilder.Configurations.Add(new InsuranceMaping());
            modelBuilder.Configurations.Add(new VehicleMapping());
            modelBuilder.Configurations.Add(new ContractMapping());
            modelBuilder.Configurations.Add(new IncidentMapping());
            modelBuilder.Configurations.Add(new RiskNoteMapping());
            modelBuilder.Configurations.Add(new RiskClaimApprovalMapping());
            modelBuilder.Configurations.Add(new RiskNoteTypesMap());
            modelBuilder.Configurations.Add(new RiskPoliceAgencyMap());
            modelBuilder.Configurations.Add(new RiskDamageMapping());
            modelBuilder.Configurations.Add(new RiskDamageTypeMap());
            modelBuilder.Configurations.Add(new RiskFilesMapping());
            modelBuilder.Configurations.Add(new RiskFileTypesMapping());
            modelBuilder.Configurations.Add(new RiskDriverInsuranceMap());
            modelBuilder.Configurations.Add(new RiskPaymentMapping());
            modelBuilder.Configurations.Add(new RiskPaymentTypeMapping());
            modelBuilder.Configurations.Add(new RiskBillingMapping());
            modelBuilder.Configurations.Add(new RiskAdminChargeMapping());
            modelBuilder.Configurations.Add(new UserActionLogMapping());
            modelBuilder.Configurations.Add(new RiskCategoryMapping());
            modelBuilder.Configurations.Add(new PermissionLevelMapping());
            modelBuilder.Configurations.Add(new RiskDocumentCategoryMap());
            modelBuilder.Configurations.Add(new RiskDocumentTypeMap());
            modelBuilder.Configurations.Add(new RiskReportMapping());
            modelBuilder.Configurations.Add(new RiskReportCategoryMapping());
            modelBuilder.Configurations.Add(new SliMapping());
            modelBuilder.Configurations.Add(new NonContractMapping());
            modelBuilder.Configurations.Add(new RiskReportDefaultselectionMapping());
            modelBuilder.Configurations.Add(new RiskDocumentsReceivedMapping());
            modelBuilder.Configurations.Add(new RiskTrackingMap());
            modelBuilder.Configurations.Add(new ClaimTrackingsMap());
            modelBuilder.Configurations.Add(new RiskTrackingTypesMap());
            modelBuilder.Configurations.Add(new RiskWriteOffTypeMapping());
            modelBuilder.Configurations.Add(new RiskWriteOffMapping());
            modelBuilder.Configurations.Add(new RiskPaymentReasonMapping());
            
        }
    
    
    }
}

