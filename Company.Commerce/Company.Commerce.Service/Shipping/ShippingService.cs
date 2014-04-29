using Company.Commerce.Entity.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Company.Commerce.Service
{
    public class ShippingService : IShippingService
    {
        public Decimal CalculateShipping(IEnumerable<OrderLine> orderLines)
        {
            if (orderLines == null)
                throw new ArgumentNullException("orderLines");

            if (!orderLines.Any())
                throw new InvalidOperationException("Empty orderLines collection.");

            //TODO Possibly look up actual shipping costs from USPS or another provider
            return 5.00m;
        }
    }
}
