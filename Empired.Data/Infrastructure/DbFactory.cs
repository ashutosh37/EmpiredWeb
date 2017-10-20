using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Empired.Data.Infrastructure
{
    public class DbFactory : Disposable, IDbFactory
    {
        RWRSPContext dbContext;

        public RWRSPContext Init()
        {
            return dbContext ?? (dbContext = new RWRSPContext());
        }

        protected override void DisposeCore()
        {
            if (dbContext != null)
                dbContext.Dispose();
        }
    }
}
