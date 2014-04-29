using Project.MVC.Entity.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Project.MVC.Web.ViewModels.Checkout.Steps
{
    [Bind(Exclude="CartItems")]
    public abstract class CheckoutBaseViewModel
    {
        public String CheckoutInstance { get; set; }

        //Not bound
        public IEnumerable<CartItem> CartItems { get; set; }
    }
}