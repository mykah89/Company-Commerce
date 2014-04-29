using Company.Commerce.Entity.Models;
using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Company.Commerce.Web.ViewModels.Shared.Validators
{
    public class FluentAddressViewModelValidator : AbstractValidator<AddressViewModel>
    {
        public FluentAddressViewModelValidator()
        {
            RuleFor(t => t.AddressLine1).NotEmpty().WithMessage("Address line is required.");

            RuleFor(t => t.City).NotEmpty().WithMessage("City is required.");

            RuleFor(t => t.PostalCode).NotEmpty().WithMessage("Postal code is required.");

            RuleFor(t => t.State).NotEmpty().WithMessage("State is required.")
                .Must(BeValidStateCode).WithMessage("Invalid selection.");
        }

        private Boolean BeValidStateCode(StateCode? stateCode)
        {
            if (!stateCode.HasValue)
                return false;

            IEnumerable<StateCode> stateCodeVals = Enum.GetValues(typeof(StateCode)).Cast<StateCode>();

            //State code must be within the range of the enumeration, that is between 0 and the number of items
            return stateCode.Value > 0 
                ? (Int32)stateCode.Value < stateCodeVals.Count() - 1 : false;
        }
    }
}