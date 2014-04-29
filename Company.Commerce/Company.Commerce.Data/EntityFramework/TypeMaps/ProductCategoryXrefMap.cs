using Company.Commerce.Entity.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity.ModelConfiguration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Company.Commerce.Data.EntityFramework.TypeMaps
{
    internal class ProductCategoryXrefMap : EntityTypeConfiguration<ProductCategoryXref>
    {
        public ProductCategoryXrefMap()
        {
            HasKey(t => new { t.ProductId, t.CategoryId });

            HasRequired(t => t.Product).WithMany(t => t.AssociatedCategories)
                .HasForeignKey(t => t.ProductId);

            HasRequired(t => t.Category).WithMany(t => t.AssociatedProducts)
                .HasForeignKey(t => t.CategoryId);

            ToTable("ProductCategoryXref");
        }
    }
}
