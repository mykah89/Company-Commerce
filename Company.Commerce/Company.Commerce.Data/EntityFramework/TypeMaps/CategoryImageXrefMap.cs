using Company.Commerce.Entity.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity.ModelConfiguration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Company.Commerce.Data.EntityFramework.TypeMaps
{
    internal class CategoryImageXrefMap : EntityTypeConfiguration<CategoryImageXref>
    {
        public CategoryImageXrefMap()
        {
            HasKey(t => new { t.CategoryId, t.ImageId });

            HasRequired(t => t.Category).WithMany(t => t.AssociatedImages)
                .HasForeignKey(t => t.CategoryId);

            HasRequired(t => t.Image).WithMany(t => t.AssociatedCategories)
                .HasForeignKey(t => t.ImageId);

            ToTable("CategoryImageXref");
        }
    }
}
