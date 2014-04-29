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
    public class OrderLineMap : EntityTypeConfiguration<OrderLine>
    {
        public OrderLineMap()
        {
            HasKey(t => t.OrderLineId)
                .Property(t => t.OrderLineId)
                .HasDatabaseGeneratedOption(DatabaseGeneratedOption.Identity);

            HasRequired(t => t.Order)
                .WithMany(t => t.OrderLines)
                .HasForeignKey(t => t.OrderId);

            Property(t => t.Price).IsRequired().HasPrecision(7, 2);

            HasRequired(t => t.Product);

            Property(t => t.Quantity).IsRequired();
        }
    }
}
