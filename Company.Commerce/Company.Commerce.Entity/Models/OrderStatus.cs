using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Company.Commerce.Entity.Models
{
    public enum OrderStatus
    {
        Created,
        Processing,
        Paid,
        Shipped,
        Returned,
        Refunded
    }
}
