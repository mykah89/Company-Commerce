using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Company.Commerce.Entity.Models
{
    public class Product
    {
        public Product()
        {
            AssociatedImages = new List<ProductImageXref>();
            AssociatedCategories = new List<ProductCategoryXref>();
        }

        public Boolean IsActive { get; set; }

        public String Name { get; set; }

        [Range(typeof(Decimal), "0", "10000", ErrorMessage = "{0} must fall between {1} and {2}.")]
        public Decimal Price { get; set; }

        public Int32 ProductId { get; set; }

        [Range(typeof(Int32), "0", "10000", ErrorMessage = "{0} must fall between {1} and {2}.")]
        public Int32 UnitsInStock { get; set; }

        public virtual IList<ProductCategoryXref> AssociatedCategories { get; set; }

        public virtual IList<ProductImageXref> AssociatedImages { get; set; }
    }
}
