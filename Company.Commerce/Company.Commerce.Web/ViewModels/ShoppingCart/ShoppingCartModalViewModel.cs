using Company.Commerce.Entity.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Company.Commerce.Web.ViewModels.ShoppingCart
{
    public class ShoppingCartModalViewModel
    {
        public ShoppingCartModalViewModel()
        {
            CartItems = new List<CartItem>();
        }

        public IEnumerable<CartItem> CartItems { get; set; }

        public Int32 ItemCount
        {
            get
            {
                return CartItems.Sum(c => c.Quantity);
            }
        }

        public decimal Price
        {
            get
            {
                return CartItems.Sum(c => c.Quantity * c.Product.Price);
            }
        }
    }
}