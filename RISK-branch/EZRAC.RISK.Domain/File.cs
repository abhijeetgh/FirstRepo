namespace EZRAC.RISK.Domain
{
    using System;
    using System.Collections.Generic;

    public class RiskFile 
    {
        public int Id{ get; set; }
        public string FileName { get; set; }
        public string FilePath{ get; set; }
        public RiskFileTypes FileType { get; set; }
        public int FileTypeId{ get; set; }
        public Int64 ClaimId { get; set; }
        public Claim Claim { get; set; }
    }
}
