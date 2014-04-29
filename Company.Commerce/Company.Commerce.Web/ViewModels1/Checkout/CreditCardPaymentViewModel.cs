using FluentValidation.Attributes;
using Project.MVC.Web.Validators.Checkout;
using Project.MVC.Web.ViewModels.Checkout;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Project.MVC.Web.ViewModels.Checkout
{
    //[Validator(typeof(CreditCardPaymentViewModelValidator))]
    public class CreditCardPaymentViewModel : PaymentViewModel
    {
        public CreditCardPaymentViewModel()
        {
            this.PaymentMethod = Entity.Models.PaymentMethod.CreditCard;
        }

        [Display(Name = "Card #")]
        public String CreditCardNumber { get; set; }

        public Int32 ExpirationMonth { get; set; }

        public Int32 ExpirationYear { get; set; }

        [Display(Name = "Cvv")]
        public String CVV { get; set; }
    }
}