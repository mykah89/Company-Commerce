using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Company.Commerce.Entity.Models
{
    public class State
    {
        public String Name { get; set; }

        public String StateCode { get; set; }

        public Int32 StateId { get; set; }

        public decimal TaxRate { get; set; }
    }
}
