using Company.Commerce.Entity.Models;
using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Company.Commerce.Service.Validation.FluentValidation
{
    public class FluentCartItemValidator : AbstractValidator<CartItem>
    {
        private readonly IProductService _productService;

        public FluentCartItemValidator(IProductService productService)
        {
            _productService = productService;

            this.CascadeMode = CascadeMode.StopOnFirstFailure;

            RuleFor(t => t.ProductId).NotEmpty().Must(ReferToAValidProduct);

            RuleFor(t => t.Quantity).NotEmpty().Must(NotBeGreaterThanProductStock).WithMessage("Quantity exceeds the product amount available.");

            RuleFor(t => t.ShoppingCartId).NotEmpty();
        }

        private Boolean NotBeGreaterThanProductStock(CartItem instance, Int32 quantity)
        {
            Product product = _productService.Get(instance.ProductId);

            return product.UnitsInStock >= instance.Quantity;
        }

        private Boolean ReferToAValidProduct(Int32 productId)
        {
            return _productService.Get(productId) != null;
        }
    }
}
