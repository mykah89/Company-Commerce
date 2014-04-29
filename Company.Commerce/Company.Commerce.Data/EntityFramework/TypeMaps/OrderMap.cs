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
    public class OrderMap : EntityTypeConfiguration<Order>
    {
        public OrderMap()
        {
            HasKey(t => t.OrderId);

            Property(t => t.OrderId).HasDatabaseGeneratedOption(DatabaseGeneratedOption.Identity);

            HasMany(t => t.OrderLines)
                .WithRequired(t => t.Order)
                .HasForeignKey(t => t.OrderId);

            Property(t => t.OrderDate).IsRequired();
        }
    }
}
