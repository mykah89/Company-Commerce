using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Company.Commerce.Entity.Models
{
    public class ProductImageXref
    {
        public Int32 ImageId { get; set; }

        public Boolean IsDefault { get; set; }

        public Int32 ProductId { get; set; }

        public virtual Image Image { get; set; }

        public virtual Product Product { get; set; }
    }
}
