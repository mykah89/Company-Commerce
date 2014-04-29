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
    internal class ProductMap : EntityTypeConfiguration<Product>
    {
        public ProductMap()
        {
            HasKey(t => t.ProductId);

            Property(t => t.ProductId).HasDatabaseGeneratedOption(DatabaseGeneratedOption.Identity);

            Property(t => t.Name).IsRequired().HasMaxLength(60);

            Property(t => t.Price).IsRequired().HasPrecision(7, 2);

            Property(t => t.UnitsInStock).IsRequired();
        }
    }
}
