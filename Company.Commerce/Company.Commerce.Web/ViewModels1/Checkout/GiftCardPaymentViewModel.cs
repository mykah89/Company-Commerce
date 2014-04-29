using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using FluentValidation.Attributes;
using System.Web.Mvc;
using Project.MVC.Web.Validators.Checkout;
using Project.MVC.Web.Binders.Checkout;

namespace Project.MVC.Web.ViewModels.Checkout
{
    [Bind(Exclude = "Balance, GiftCardID")]
    [ModelBinder(typeof(GiftCardPaymentViewModelBinder))]
    public class GiftCardPaymentViewModel : PaymentViewModel
    {
        //Not bound, bound during model binding
        public Int32 GiftCardID { get; set; }

        public GiftCardPaymentViewModel()
        {
            this.PaymentMethod = Entity.Models.PaymentMethod.GiftCard;
        }

        [Display(Name = "Card #")]
        public String GiftCardNumber { get; set; }
        [Display(Name = "Pin")]
        public String GiftCardPin { get; set; }

        //Not bound from view, bound from modelbinder
        public decimal Balance { get; set; }
    }
}