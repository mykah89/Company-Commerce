using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Company.Commerce.Entity.Models
{
    public class ProductCategoryXref
    {
        public Int32 CategoryId { get; set; }

        public Int32 ProductId { get; set; }

        public Boolean IsDefault { get; set; }

        public virtual Category Category { get; set; }

        public virtual Product Product { get; set; }
    }
}
