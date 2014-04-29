using Company.Commerce.Entity.Models;
using Company.Commerce.Web.ViewModels.Shared;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Company.Commerce.Web.ViewModels.Checkout
{
    public class CheckoutViewModel
    {
        public CheckoutViewModel()
        {
            OrderLines = new List<OrderLine>();
        }

        public AddressViewModel BillingAddress { get; set; }

        public CreditCardViewModel CreditCard { get; set; }

        public String EmailAddress { get; set; }

        public String InstanceKey { get; set; }

        public String PhoneNumber { get; set; }

        public Decimal ShippingCost { get; set; }

        public AddressViewModel ShippingAddress { get; set; }

        public IList<OrderLine> OrderLines { get; set; }

        public Decimal SalesTax { get; set; }
    }
}