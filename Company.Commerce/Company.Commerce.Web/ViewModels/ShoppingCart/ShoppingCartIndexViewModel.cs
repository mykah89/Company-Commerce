using Company.Commerce.Entity.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Company.Commerce.Web.ViewModels.ShoppingCart
{
    public class ShoppingCartIndexViewModel
    {
        public ShoppingCartIndexViewModel()
        {
            CartItems = new List<CartItem>();
            CartUpdates = new List<String>();
            PromoCodes = new List<String>();
        }

        public List<CartItem> CartItems { get; set; }

        public List<String> CartUpdates { get; set; }

        public String PostalCode { get; set; }

        public List<String> PromoCodes { get; set; }

        public decimal ShippingCost { get; set; }

        public decimal ShoppingCartTotalPrice { get; set; }
    }
}