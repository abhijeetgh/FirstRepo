namespace EZRAC.RISK.Domain
{
    using System;

    public abstract class AuditableEntity : BaseEntity
    {
        public long CreatedBy { get; set; }

        public long UpdatedBy { get; set; }

        public System.DateTime CreatedDateTime { get; set; }

        public System.DateTime UpdatedDateTime { get; set; }


        public void FillAuditableEntity(long userId, bool IsCreate)
        {
            if (IsCreate)
            {
                this.CreatedBy = userId;
                this.UpdatedBy = userId;
                this.CreatedDateTime = DateTime.Now;
                this.UpdatedDateTime = DateTime.Now;
            }
            else {

                this.UpdatedBy = userId;
                this.UpdatedDateTime = DateTime.Now;
            }

         

        }

    }
}
