using Project.MVC.Entity.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Project.MVC.Web.ViewModels.Checkout
{
    public class PaymentViewModel
    {
        public PaymentMethod PaymentMethod { get; protected set; }
    }
}