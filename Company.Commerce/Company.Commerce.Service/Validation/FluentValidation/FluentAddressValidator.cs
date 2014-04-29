using Company.Commerce.Entity.Models;
using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Company.Commerce.Service.Validation.FluentValidation
{
    public class FluentAddressValidator : AbstractValidator<Address>
    {
        public FluentAddressValidator()
        {
            RuleSet("Create", () =>
            {
                RuleFor(t => t.AddressLine1).NotEmpty().WithMessage("Address line is required.");

                RuleFor(t => t.City).NotEmpty().WithMessage("City is required.");

                RuleFor(t => t.PostalCode).NotEmpty().WithMessage("Postal code is required.");

                RuleFor(t => t.State).NotEmpty().WithMessage("State is required.")
                    .Must(ReferToAValidState).WithMessage("Invalid state.");
            });
        }

        private Boolean ReferToAValidState(String state)
        {
            throw new NotImplementedException();
        }
    }
}
