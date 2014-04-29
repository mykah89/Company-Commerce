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
    public class FluentOrderLineValidator : AbstractValidator<OrderLine>
    {
        private readonly IProductService _productService;

        public FluentOrderLineValidator(IProductService productService)
        {
            _productService = productService;

            this.CascadeMode = CascadeMode.StopOnFirstFailure;

            RuleFor(t => t.Price).NotEmpty().WithMessage("Price is required.")
                .Must(ContainTwoDecimalPlaces).WithMessage("Price format is invalid. {e.g X.00}"); ;

            RuleFor(t => t.ProductId).NotEmpty().WithMessage("ProductId is required.")
                .Must(ReferToAValidProduct).WithMessage("ProductId does not refer to a valid product.");

            RuleFor(t => t.Quantity).NotEmpty().WithMessage("Quantity is required.")
                .GreaterThan(0).WithMessage("Quantity must be greater than 0.");
        }

        private Boolean ContainTwoDecimalPlaces(Decimal price)
        {
            String pattern = @"^[0-9]*\.[0-9]{2}$";// or ^[0-9]*\.[0-9][0-9]$";

            Regex regex = new Regex(pattern);

            return regex.IsMatch(price.ToString());
        }

        private Boolean ReferToAValidProduct(Int32 productId)
        {
            return _productService.Get(productId) != null;
        }
    }
}
