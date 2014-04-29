using Project.MVC.Entity.Models;
using Project.MVC.Web.ViewModels.ShoppingCart;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Project.MVC.Web.ViewModels.ShoppingCart
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
                return CartItems.Sum(c => c.Quantity * c.Product.UnitPrice);
            }
        }
    }
}