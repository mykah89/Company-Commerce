using Company.Commerce.Entity.Models;
using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FluentValidation.Resources;
using FluentValidation.Results;

namespace Company.Commerce.Service.Validation.FluentValidation
{
    public class FluentOrderValidator : AbstractValidator<Order>
    {
        private readonly IProductService _productService;

        public FluentOrderValidator(IProductService productService)
        {
            _productService = productService;

            this.CascadeMode = CascadeMode.StopOnFirstFailure;

            RuleSet("Create", () =>
                {
                    RuleFor(t => t.OrderDate).NotEmpty().WithMessage("OrderDate is required.")
                        .LessThanOrEqualTo(DateTime.Now + TimeSpan.FromMinutes(1)).WithMessage("Order date must not be in the future.");

                    RuleFor(t => t.OrderLines).NotEmpty().WithMessage("Order requires orderlines.")
                        .Must(BeValidOrderLines).WithMessage("Order contains invalid orderlines.");

                    RuleFor(t => t.OrderStatus).Equal(OrderStatus.Created).WithMessage("Order status must be 'Created'.");
                });
        }

        private Boolean BeValidOrderLines(ICollection<OrderLine> orderLines)
        {
            FluentOrderLineValidator validator = new FluentOrderLineValidator(_productService);

            foreach (var ol in orderLines)
            {
                ValidationResult result = validator.Validate(ol);

                if (!result.IsValid)
                    return false;
            }

            return true;
        }
    }
}
