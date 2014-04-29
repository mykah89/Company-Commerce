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
    internal class CartItemMap : EntityTypeConfiguration<CartItem>
    {
        public CartItemMap()
        {
            Property(t => t.CartItemId).HasDatabaseGeneratedOption(DatabaseGeneratedOption.Identity);

            Property(t => t.ShoppingCartId).IsRequired();

            HasRequired(t => t.Product).WithMany();
        }
    }
}
