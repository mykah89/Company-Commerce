using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Company.Commerce.Entity.Models
{
    public class Image
    {
        public Image()
        {
            AssociatedCategories = new List<CategoryImageXref>();
            AssociatedProducts = new List<ProductImageXref>();
        }

        public Int32 ImageId { get; set; }

        public String ImagePath { get; set; }

        public virtual IList<CategoryImageXref> AssociatedCategories { get; set; }

        public virtual IList<ProductImageXref> AssociatedProducts { get; set; }
    }
}
