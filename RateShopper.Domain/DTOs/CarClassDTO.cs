using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RateShopper.Domain.DTOs
{
    public class CarClassDTO
    {
        public long ID { get; set; }
        public string Code { get; set; }
        public string Description { get; set; }
        public string Logo { get; set; }
        public long CreatedBy { get; set; }
        public int CarClassOrder { get; set; }
    }
    public class CarClassMasterDTO
    {
        public long ID { get; set; }
        public string Code { get; set; }
        public string Description { get; set; }
        public string Logo { get; set; }
        public bool IsDeleted { get; set; }
        public long CreatedBy { get; set; }
        public long UpdatedBy { get; set; }
        public System.DateTime CreatedDateTime { get; set; }
        public System.DateTime UpdatedDateTime { get; set; }
        public int CarClassOrder { get; set; }
    }
    public class CarClassCodeOrderDTO
    {
        public string CarClassCode { get; set; }
        public int DisplayOrder { get; set; }
    }
}
