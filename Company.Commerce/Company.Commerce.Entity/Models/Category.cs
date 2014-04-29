using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Company.Commerce.Entity.Models
{
    public class Category
    {
        public Category()
        {
            AssociatedImages = new List<CategoryImageXref>();
            AssociatedProducts = new List<ProductCategoryXref>();
            ChildCategories = new List<Category>();
        }

        public Int32 CategoryId { get; set; }

        public String Name { get; set; }

        public Int32? ParentCategoryId { get; set; }

        public virtual IList<CategoryImageXref> AssociatedImages { get; set; }

        public virtual IList<ProductCategoryXref> AssociatedProducts { get; set; }

        public virtual IList<Category> ChildCategories { get; set; }

        public virtual Category ParentCategory { get; set; }
    }
}
