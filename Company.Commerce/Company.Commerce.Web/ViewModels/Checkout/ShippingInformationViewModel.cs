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
    [Validator(typeof(FluentShippingInformationViewModelValidator))]
    public class ShippingInformationViewModel : CheckoutStepBase
    {
        public ShippingInformationViewModel()
        {
            StateCodes = Enum.GetValues(typeof(StateCode)).Cast<StateCode>();
            OrderLines = new List<OrderLine>();
        }

        public AddressViewModel ShippingAddress { get; set; }

        [Display(Name = "Email")]
        public String EmailAddress { get; set; }

        [Display(Name = "Phone")]
        public String PhoneNumber { get; set; }

        public Decimal SalesTax { get; set; }

        public IEnumerable<StateCode> StateCodes { get; set; }

        public IEnumerable<OrderLine> OrderLines { get; set; }
    }
}