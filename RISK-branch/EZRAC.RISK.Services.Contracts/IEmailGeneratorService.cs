using EZRAC.RISK.Services.Contracts.Dtos;
using EZRAC.RISK.Util.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EZRAC.RISK.Services.Contracts
{
    public interface IEmailGeneratorService
    {
        Task<IEnumerable<InformationToShowDto>> GetInformationToShowList(long claimId);

        Task<IEnumerable<EmailGeneratorReceipientDto>> GetRecipients(long claimId);

        Task<InformationToSendDto> GetInformationToSend(ClaimsConstant.EmailInfoToSend[] SelectedInfoToSend, long claimId);

        Task<IEnumerable<string>> GetEmailRecipients(EmailGeneratorDto emailGeneratorDto);

        Task<string> GetSubjectLine(long claimId, string LoggedInUserName);
    }
}
