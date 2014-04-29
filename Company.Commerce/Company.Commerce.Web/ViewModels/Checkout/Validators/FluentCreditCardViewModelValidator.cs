using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Company.Commerce.Web.ViewModels.Checkout.Validators
{
    public class FluentCreditCardViewModelValidator : AbstractValidator<CreditCardViewModel>
    {
        public FluentCreditCardViewModelValidator()
        {
            RuleFor(t => t.CardNumber).NotEmpty().WithMessage("Card number is required.")
                .CreditCard();

            RuleFor(t => t.ExpirationMonth).NotEmpty().Must(BeGreaterOrEqualToTheCurrentMonth).Unless(ExpirationYearIsGreaterThanThisYear);

            RuleFor(t => t.ExpirationYear).NotEmpty().Must(BeGreaterThanOrEqualToTheCurrentYear)
                .Must(BeGreaterThanTheCurrentYear)
                .When(ExpirationMonthIsLessThanCurrentMonth);

            RuleFor(t => t.VerificationNumber).NotEmpty().WithMessage("Verification number is required.")
                .Length(3, 4);
        }

        private Boolean BeGreaterThanOrEqualToTheCurrentYear(DateTime? expirationYear)
        {
            if (!expirationYear.HasValue)
                return false;

            return expirationYear.Value.Year >= DateTime.Now.Year;
        }

        private Boolean ExpirationMonthIsLessThanCurrentMonth(CreditCardViewModel instance)
        {
            if (!instance.ExpirationMonth.HasValue)
                return true;

            return (Int32)instance.ExpirationMonth.Value < DateTime.Now.Month;
        }

        private bool BeGreaterThanTheCurrentYear(DateTime? expirationYear)
        {
            if (!expirationYear.HasValue)
                return false;

            return expirationYear.Value.Year > DateTime.Now.Year;
        }

        private Boolean ExpirationYearIsGreaterThanThisYear(CreditCardViewModel instance)
        {
            if (!instance.ExpirationYear.HasValue)
                return false;

            return instance.ExpirationYear.Value.Year > DateTime.Now.Year;
        }

        private Boolean BeGreaterOrEqualToTheCurrentMonth(Month? month)
        {
            if (!month.HasValue)
                return false;

            return (Int32)month.Value >= DateTime.Now.Month;
        }
    }
}