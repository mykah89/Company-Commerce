using Company.Commerce.Entity.Models;
using Company.Commerce.Web.ViewModels.Checkout.Validators;
using Company.Commerce.Web.ViewModels.Shared;
using FluentValidation.Attributes;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Company.Commerce.Web.ViewModels.Checkout
{
    [Validator(typeof(FluentBillingInformationViewModelValidator))]
    public class BillingInformationViewModel : CheckoutStepBase
    {
        public BillingInformationViewModel()
        {
            StateCodes = Enum.GetValues(typeof(StateCode)).Cast<StateCode>();
            OrderLines = new List<OrderLine>();
        }

        public AddressViewModel BillingAddress { get; set; }

        [Display(Name = "Same as Shipping")]
        public Boolean BillingSameAsShipping { get; set; }

        public CreditCardViewModel CreditCard { get; set; }

        public Decimal SalesTax { get; set; }

        public AddressViewModel ShippingAddress { get; set; }

        public Decimal ShippingCost { get; set; }

        public IEnumerable<StateCode> StateCodes { get; set; }

        public IEnumerable<OrderLine> OrderLines { get; set; }

    }
}