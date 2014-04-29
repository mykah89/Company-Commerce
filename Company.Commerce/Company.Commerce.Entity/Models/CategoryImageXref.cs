using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Company.Commerce.Entity.Models
{
    public class CategoryImageXref
    {
        public Int32 CategoryId { get; set; }

        public Int32 ImageId { get; set; }

        public virtual Category Category { get; set; }

        public virtual Image Image { get; set; }
    }
}
