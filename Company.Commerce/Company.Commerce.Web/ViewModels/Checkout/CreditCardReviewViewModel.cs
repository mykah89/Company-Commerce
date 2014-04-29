using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Company.Commerce.Web.ViewModels.Checkout
{
    public class CreditCardReviewViewModel
    {
        public String MaskedCardNumber { get; set; }

        public Month ExpirationMonth { get; set; }

        public String ExpirationYear { get; set; }

        public String MaskedCVV { get; set; }
    }
}