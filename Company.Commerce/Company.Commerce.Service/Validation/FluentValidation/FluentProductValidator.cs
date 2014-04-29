using Company.Commerce.Entity.Models;
using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Company.Commerce.Service.Validation.FluentValidation
{
    public class FluentProductValidator : AbstractValidator<Product>
    {
        public FluentProductValidator()
        {
            RuleFor(t => t.Name).NotEmpty().WithMessage("Name is required.")
                .Length(6, 40);

            RuleFor(t => t.Price).NotEmpty().WithMessage("Price is required.")
                .Must(ContainTwoDecimalPlaces).WithMessage("Price format is invalid. {e.g 1.00}");

            RuleFor(t => t.UnitsInStock).GreaterThanOrEqualTo(0).WithMessage("Units in stock must not be less than 0.");
        }

        private Boolean ContainTwoDecimalPlaces(Decimal price)
        {
            String pattern = @"^[0-9]*\.[0-9]{2}$";// or ^[0-9]*\.[0-9][0-9]$";

            Regex regex = new Regex(pattern);

            return regex.IsMatch(price.ToString());
        }
    }
}
