using Project.MVC.Entity.Models;
using Project.MVC.Web.ViewModels.Checkout;
using Project.MVC.Web.ViewModels.General;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Project.MVC.Web.ViewModels.Checkout.Steps
{
    public sealed class ReviewViewModel : CheckoutBaseViewModel
    {
        public Order Order { get; set; }

        public CreditCardPaymentViewModel CreditCardPayment { get; set; }
    }
}