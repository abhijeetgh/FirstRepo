using EZRAC.Risk.UI.Web.App_Start;
using EZRAC.Risk.UI.Web.DocumentGenerator.Implementation;
using EZRAC.Risk.UI.Web.Helper;
using EZRAC.RISK.Services.Contracts;
using EZRAC.RISK.Util.Common;
using Microsoft.Practices.Unity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace EZRAC.Risk.UI.Web.DocumentGenerator
{
    public static class DocumentFactory
    {
        public static IDocumentGenerator GetInstance(DocumentTypes documentType)
        {

            IDocumentGenerator documentGenerator = null;

            switch (documentType)
            {
                case DocumentTypes.AR_DL1_RTR:
                    documentGenerator = UnityConfig.GetConfiguredContainer().Resolve<IDocumentGenerator>(DocumentTypes.AR_DL1_RTR.ToString());//container.Resolve<IDocumentGenerator>("Ardl1");
                    break;
                case DocumentTypes.AR_DL2_RTR:
                    documentGenerator = UnityConfig.GetConfiguredContainer().Resolve<IDocumentGenerator>(DocumentTypes.AR_DL1_RTR.ToString());
                    break;
                case DocumentTypes.AR_DL3_RTR:
                    documentGenerator = UnityConfig.GetConfiguredContainer().Resolve<IDocumentGenerator>(DocumentTypes.AR_DL1_RTR.ToString());
                    break;
                case DocumentTypes.Introduction_Letter:
                    documentGenerator = UnityConfig.GetConfiguredContainer().Resolve<IDocumentGenerator>(DocumentTypes.Introduction_Letter.ToString());
                    break;
                case DocumentTypes.JNR_Debtor_Placement_Form:
                    documentGenerator = UnityConfig.GetConfiguredContainer().Resolve<IDocumentGenerator>(DocumentTypes.JNR_Debtor_Placement_Form.ToString());
                    break;
                case DocumentTypes.National_Casualty_Insurance_Cover:
                    documentGenerator = UnityConfig.GetConfiguredContainer().Resolve<IDocumentGenerator>(DocumentTypes.National_Casualty_Insurance_Cover.ToString());
                    break;
                case DocumentTypes.Zurich_Empire_Letter:
                    documentGenerator = UnityConfig.GetConfiguredContainer().Resolve<IDocumentGenerator>(DocumentTypes.National_Casualty_Insurance_Cover.ToString());
                    break;
                case DocumentTypes.Attorney_Acknowledgement_Letter:
                    documentGenerator = UnityConfig.GetConfiguredContainer().Resolve<IDocumentGenerator>(DocumentTypes.Attorney_Acknowledgement_Letter.ToString());
                    break;
                case DocumentTypes.CDW_NotVoided_ULL:
                    documentGenerator = UnityConfig.GetConfiguredContainer().Resolve<IDocumentGenerator>(DocumentTypes.CDW_NotVoided_ULL.ToString());
                    break;
                case DocumentTypes.Client_Info_and_Authorization:
                    documentGenerator = UnityConfig.GetConfiguredContainer().Resolve<IDocumentGenerator>(DocumentTypes.Client_Info_and_Authorization.ToString());
                    break;
                case DocumentTypes.Customer_Fax_Cover_Sheet:
                    documentGenerator = UnityConfig.GetConfiguredContainer().Resolve<IDocumentGenerator>(DocumentTypes.Customer_Fax_Cover_Sheet.ToString());
                    break;
                case DocumentTypes.Customer_Service_Information:
                    documentGenerator = UnityConfig.GetConfiguredContainer().Resolve<IDocumentGenerator>(DocumentTypes.Customer_Service_Information.ToString());
                    break;
                case DocumentTypes.Demand_Letter_1_3rd_Party:
                    documentGenerator = UnityConfig.GetConfiguredContainer().Resolve<IDocumentGenerator>(DocumentTypes.Demand_Letter_1_3rd_Party.ToString());
                    break;
                case DocumentTypes.Payment_Balance_due:
                    documentGenerator = UnityConfig.GetConfiguredContainer().Resolve<IDocumentGenerator>(DocumentTypes.Payment_Balance_due.ToString());
                    break;
                case DocumentTypes.Payment_Claim_Closed:
                    documentGenerator = UnityConfig.GetConfiguredContainer().Resolve<IDocumentGenerator>(DocumentTypes.Payment_Balance_due.ToString());
                    break;
                case DocumentTypes.Payment_Plan:
                    documentGenerator = UnityConfig.GetConfiguredContainer().Resolve<IDocumentGenerator>(DocumentTypes.Payment_Plan.ToString());
                    break;
                case DocumentTypes.Demand_Letter_1_3rd_Party_Insurance:
                    documentGenerator = UnityConfig.GetConfiguredContainer().Resolve<IDocumentGenerator>(DocumentTypes.Demand_Letter_1_3rd_Party_Insurance.ToString());
                    break;
                case DocumentTypes.Payoff_Form:
                    documentGenerator = UnityConfig.GetConfiguredContainer().Resolve<IDocumentGenerator>(DocumentTypes.Payoff_Form.ToString());
                    break;
                case DocumentTypes.Repo_Auth_Nationwide_Repo:
                    documentGenerator = UnityConfig.GetConfiguredContainer().Resolve<IDocumentGenerator>(DocumentTypes.Repo_Auth_Nationwide_Repo.ToString());
                    break;
                case DocumentTypes.Demand_Letter_1_Renter_Insurance:
                    documentGenerator = UnityConfig.GetConfiguredContainer().Resolve<IDocumentGenerator>(DocumentTypes.Demand_Letter_1_3rd_Party_Insurance.ToString());
                    break;
                case DocumentTypes.Demand_Letter_1_Renter:
                    documentGenerator = UnityConfig.GetConfiguredContainer().Resolve<IDocumentGenerator>(DocumentTypes.Demand_Letter_1_Renter.ToString());
                    break;
                case DocumentTypes.Demand_Letter_2_Renter:
                    documentGenerator = UnityConfig.GetConfiguredContainer().Resolve<IDocumentGenerator>(DocumentTypes.Demand_Letter_1_Renter.ToString());
                    break;
                case DocumentTypes.Demand_Letter_3_Renter:
                    documentGenerator = UnityConfig.GetConfiguredContainer().Resolve<IDocumentGenerator>(DocumentTypes.Demand_Letter_3_Renter.ToString());
                    break;
                case DocumentTypes.Demand_Letter_4_Renter:
                    documentGenerator = UnityConfig.GetConfiguredContainer().Resolve<IDocumentGenerator>(DocumentTypes.Demand_Letter_3_Renter.ToString());
                    break;
                case DocumentTypes.Reposession_Authorization:
                    documentGenerator = UnityConfig.GetConfiguredContainer().Resolve<IDocumentGenerator>(DocumentTypes.Reposession_Authorization.ToString());
                    break;
                case DocumentTypes.Salvage_Bid_Accepted:
                    documentGenerator = UnityConfig.GetConfiguredContainer().Resolve<IDocumentGenerator>(DocumentTypes.Salvage_Bid_Accepted.ToString());
                    break;
                case DocumentTypes.Salvage_Bid:
                    documentGenerator = UnityConfig.GetConfiguredContainer().Resolve<IDocumentGenerator>(DocumentTypes.Salvage_Bid_Accepted.ToString());
                    break;
                case DocumentTypes.GFL_Renter:
                    documentGenerator = UnityConfig.GetConfiguredContainer().Resolve<IDocumentGenerator>(DocumentTypes.GFL_Renter.ToString());
                    break;
                case DocumentTypes.Ticket_Admin_Fee:
                    documentGenerator = UnityConfig.GetConfiguredContainer().Resolve<IDocumentGenerator>(DocumentTypes.Ticket_Admin_Fee.ToString());
                    break;
                case DocumentTypes.Insurance_Fax_Cover_Sheet:
                    documentGenerator = UnityConfig.GetConfiguredContainer().Resolve<IDocumentGenerator>(DocumentTypes.Insurance_Fax_Cover_Sheet.ToString());
                    break;
                case DocumentTypes.Ticket_Affidavit:
                    documentGenerator = UnityConfig.GetConfiguredContainer().Resolve<IDocumentGenerator>(DocumentTypes.Ticket_Affidavit.ToString());
                    break;
                case DocumentTypes.Vehicle_Release:
                    documentGenerator = UnityConfig.GetConfiguredContainer().Resolve<IDocumentGenerator>(DocumentTypes.Vehicle_Release.ToString());
                    break;
                case DocumentTypes.Violation_Admin_Fee:
                    documentGenerator = UnityConfig.GetConfiguredContainer().Resolve<IDocumentGenerator>(DocumentTypes.Violation_Admin_Fee.ToString());
                    break;
                case DocumentTypes.Violation_Admin_Fee_Decline:
                    documentGenerator = UnityConfig.GetConfiguredContainer().Resolve<IDocumentGenerator>(DocumentTypes.Violation_Admin_Fee.ToString());
                    break;
                case DocumentTypes.Wholesale_Bill_of_Sale:
                    documentGenerator = UnityConfig.GetConfiguredContainer().Resolve<IDocumentGenerator>(DocumentTypes.Wholesale_Bill_of_Sale.ToString());
                    break;
                case DocumentTypes.SLI_Fax_Cover_Sheet:
                    documentGenerator = UnityConfig.GetConfiguredContainer().Resolve<IDocumentGenerator>(DocumentTypes.National_Casualty_Insurance_Cover.ToString());
                    break;
                case DocumentTypes.Body_Shop_Repair:
                    documentGenerator = UnityConfig.GetConfiguredContainer().Resolve<IDocumentGenerator>(DocumentTypes.Body_Shop_Repair.ToString());
                    break;
                case DocumentTypes.Reported_Stolen:
                    documentGenerator = UnityConfig.GetConfiguredContainer().Resolve<IDocumentGenerator>(DocumentTypes.Reported_Stolen.ToString());
                    break;
                default:
                    break;
            }
            return documentGenerator;
        }
    }
}