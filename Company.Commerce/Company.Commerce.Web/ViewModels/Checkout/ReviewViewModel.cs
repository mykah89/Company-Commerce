using Company.Commerce.Entity.Models;
using Company.Commerce.Web.ViewModels.Shared;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Company.Commerce.Web.ViewModels.Checkout
{
    public class ReviewViewModel : CheckoutStepBase
    {
        public AddressViewModel BillingAddress { get; set; }

        public CreditCardReviewViewModel CreditCard { get; set; }

        public String EmailAddress { get; set; }

        public IEnumerable<OrderLine> OrderLines { get; set; }

        public String PhoneNumber { get; set; }

        public Decimal SalesTax { get; set; }

        public AddressViewModel ShippingAddress { get; set; }

        public Decimal ShippingCost { get; set; }
    }
}