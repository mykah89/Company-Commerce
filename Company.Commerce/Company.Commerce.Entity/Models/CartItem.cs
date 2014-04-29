using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Company.Commerce.Entity.Models
{
    public class CartItem
    {
        public Int32 CartItemId { get; set; }

        public Int32 Quantity { get; set; }

        public Int32 ProductId { get; set; }

        public String ShoppingCartId { get; set; }

        public virtual Product Product { get; set; }
    }
}
