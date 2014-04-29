using Company.Commerce.Entity.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity.ModelConfiguration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Company.Commerce.Data.EntityFramework.TypeMaps
{
    internal class ProductImageXrefMap : EntityTypeConfiguration<ProductImageXref>
    {
        public ProductImageXrefMap()
        {
            HasKey(t => new { t.ProductId, t.ImageId });

            HasRequired(t => t.Product).WithMany(t => t.AssociatedImages)
                .HasForeignKey(t => t.ProductId);

            HasRequired(t => t.Image).WithMany(t => t.AssociatedProducts)
                .HasForeignKey(t => t.ImageId);

            ToTable("ProductImageXref");
        }
    }
}
