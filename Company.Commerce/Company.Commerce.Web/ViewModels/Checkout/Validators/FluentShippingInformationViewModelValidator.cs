using Company.Commerce.Web.ViewModels.Shared.Validators;
using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Company.Commerce.Web.ViewModels.Checkout.Validators
{
    public class FluentShippingInformationViewModelValidator : AbstractValidator<ShippingInformationViewModel>
    {
        public FluentShippingInformationViewModelValidator()
        {
            RuleFor(t => t.EmailAddress).NotEmpty().WithMessage("Email is required.")
                .EmailAddress();

            //TODO Phone number format validation
            RuleFor(t => t.PhoneNumber).NotEmpty().WithMessage("Phone number is required.");
                //.Must(BeAValidPhoneNumber).WithMessage("Invalid phone number format.");

            RuleFor(t => t.ShippingAddress).SetValidator(new FluentAddressViewModelValidator());
        }
    }
}