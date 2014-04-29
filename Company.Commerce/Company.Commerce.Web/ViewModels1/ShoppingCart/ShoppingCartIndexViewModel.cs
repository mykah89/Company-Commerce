using Project.MVC.Entity.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Project.MVC.Web.ViewModels.ShoppingCart
{
    public class ShoppingCartIndexViewModel
    {
        public ShoppingCartIndexViewModel()
        {
            CartItems = new List<CartItem>();
            ImportantCartUpdates = new List<String>();
            PromoCodes = new List<String>();
        }

        public List<CartItem> CartItems { get; set; }

        public List<String> ImportantCartUpdates { get; set; }

        public String PostalCode { get; set; }

        public List<String> PromoCodes { get; set; }

        public decimal ShippingCost { get; set; }

        public decimal ShoppingCartTotalPrice { get; set; }
    }
}