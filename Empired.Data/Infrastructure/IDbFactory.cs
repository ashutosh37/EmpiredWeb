using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Empired.Data.Infrastructure
{
    public interface IDbFactory : IDisposable
    {
        RWRSPContext Init();
    }
}
