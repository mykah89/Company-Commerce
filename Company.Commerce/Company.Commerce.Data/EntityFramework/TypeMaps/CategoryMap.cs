using Company.Commerce.Entity.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Company.Commerce.Data.EntityFramework.TypeMaps
{
    internal class CategoryMap : EntityTypeConfiguration<Category>
    {
        public CategoryMap()
        {
            Property(t => t.CategoryId).HasDatabaseGeneratedOption(DatabaseGeneratedOption.Identity);

            Property(t => t.Name).IsRequired().HasMaxLength(60);

            HasOptional(t => t.ParentCategory)
                .WithMany(t => t.ChildCategories).HasForeignKey(t => t.ParentCategoryId);
        }
    }
}
