using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EZRAC.RISK.EntityFramework
{
    public class TSDContext : DbContext
    {

          #region Constructor(s)


        public TSDContext()
            : base("name=TSDContext")
        {
            this.Configuration.LazyLoadingEnabled = true;
            this.Configuration.ProxyCreationEnabled = false; 
        }       

        #endregion   

    }
}
