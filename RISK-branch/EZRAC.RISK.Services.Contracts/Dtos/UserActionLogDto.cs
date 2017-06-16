using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EZRAC.RISK.Services.Contracts.Dtos
{
    public class UserActionLogDto
    {
        public int Id { get; set; }

        public long UserId { get; set; }

        public string ContractNo { get; set; }

        public string UserName { get; set; }

        public string Name { get; set; }

        public long ClaimId { get; set; }

        public DateTime Date { get; set; }

        public string UserAction { get; set; }

        public string CompanyAbbr { get; set; }
    }
}
