using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EZRAC.RISK.Domain
{
    public class UserActionLog
    {
        public int Id { get; set; }
        
        public long UserId { get; set; }

        public long ClaimId { get; set; }

        public string UserAction { get; set; }    

        public DateTime Date { get; set; }

        public User User { get; set; }
    }
}
