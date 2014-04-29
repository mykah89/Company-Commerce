using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using FluentValidation.Attributes;
using System.Web.Mvc;
using Project.MVC.Web.ViewModels.General;
using Project.MVC.Web.ViewModels.Checkout;
using Project.MVC.Web.Validators.Checkout;
using Project.MVC.Web.ViewModels.ShoppingCart;
using Project.MVC.Web.Binders.Checkout;
using Project.MVC.Entity.Models;

namespace Project.MVC.Web.ViewModels.Checkout.Steps
{
    [Validator(typeof(BillingInformationViewModelValidator))]
    [ModelBinder(typeof(BillingInformationViewModelBinder))]
    [Bind(Exclude = "OrderTotal, ShippingAddress, ShippingCost, Tax")]
    public sealed class BillingInformationViewModel : CheckoutBaseViewModel
    {
        public BillingInformationViewModel()
        {
            CreditCardPayment = new CreditCardPaymentViewModel();
            //GiftCardPayments = new List<GiftCardPaymentViewModel>();
            ShippingAddress = new Address();
        }

        [Display(Name = "Address")]
        public String AddressLine1 { get; set; }

        [Display(Name = "Address 2")]
        public String AddressLine2 { get; set; }

        [Display(Name = "Billing Same As")]
        public Boolean BillingSameAsShipping { get; set; }

        public String City { get; set; }

        public CreditCardPaymentViewModel CreditCardPayment { get; set; }

        //public List<GiftCardPaymentViewModel> GiftCardPayments { get; set; }

        //Not bound
        public Decimal OrderTotal { get; set; }

        [Display(Name = "Zip")]
        public String PostalCode { get; set; }

        //Not bound
        public Address ShippingAddress { get; set; }

        public String State { get; set; }

        //Not bound
        public Decimal ShippingCost { get; set; }

        //Not bound
        public Decimal Tax { get; set; }
    }
}