using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Company.Commerce.Entity.Models
{
    public class OrderLine
    {
        public virtual Order Order { get; set; }

        public Int32 OrderId { get; set; }

        public Int32 OrderLineId { get; set; }

        [Range(typeof(Decimal), "0", "10000", ErrorMessage = "{0} must fall between {1} and {2}.")]
        public Decimal Price { get; set; }

        public virtual Product Product { get; set; }

        public Int32 ProductId { get; set; }

        [Range(typeof(Int32), "0", "100", ErrorMessage = "{0} must be greater than {1} or less than {2}.")]
        public Int32 Quantity { get; set; }
    }
}
