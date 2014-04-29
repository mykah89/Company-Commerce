using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Company.Commerce.Data
{
    public interface IDbEntityEntry
    {
        EntityState State { get; set; }
    }
}
