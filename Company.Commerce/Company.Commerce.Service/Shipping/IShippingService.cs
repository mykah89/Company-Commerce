using Company.Commerce.Entity.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Company.Commerce.Service
{
    public interface IShippingService
    {
        Decimal CalculateShipping(IEnumerable<OrderLine> orderLines);
    }
}
