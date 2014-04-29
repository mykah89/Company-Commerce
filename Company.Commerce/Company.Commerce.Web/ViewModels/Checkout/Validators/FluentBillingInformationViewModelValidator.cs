using Company.Commerce.Web.ViewModels.Shared.Validators;
using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Company.Commerce.Web.ViewModels.Checkout.Validators
{
    public class FluentBillingInformationViewModelValidator : AbstractValidator<BillingInformationViewModel>
    {
        public FluentBillingInformationViewModelValidator()
        {
            RuleFor(t => t.BillingAddress).SetValidator(new FluentAddressViewModelValidator())
                .When(t => t.BillingSameAsShipping == false);

            RuleFor(t => t.CreditCard).SetValidator(new FluentCreditCardViewModelValidator());
        }
    }
}