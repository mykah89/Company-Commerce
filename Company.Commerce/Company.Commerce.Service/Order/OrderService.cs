using Company.Commerce.Entity.Models;
using Company.Commerce.Repository;
using Company.Commerce.Service.Validation.FluentValidation;
using FluentValidation.Results;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FluentValidation;
using Company.Commerce.Service.Utility;
using Company.Commerce.Service.Validation;

namespace Company.Commerce.Service
{
    public class OrderService : IOrderService
    {
        private readonly FluentOrderValidator _orderValidator;

        private readonly IProductService _productService;

        private readonly IUnitOfWork _uow;

        public OrderService(IUnitOfWork uow, IProductService productService)
        {
            _productService = productService;

            _orderValidator = new FluentOrderValidator(productService);

            _uow = uow;
        }

        public async Task<ServiceOperationResult<Order>> CreateAsync(Order order)
        {
            if (order == null)
                throw new ArgumentNullException("order");

            if (order.OrderId > 0)
                throw new ArgumentOutOfRangeException("order.OrderId", order.OrderId, "Cannot create an order which already exists.");

            ServiceOperationResult<Order> result = new ServiceOperationResult<Order>();

            ValidationResult validationResult = await _orderValidator.ValidateAsync(order, ruleSet: "Create");

            if (validationResult.IsValid)
            {
                result.Data = _uow.Repository<Order>().Add(order);

                result.Succeeded = true;
            }
            else
                result.Errors.AddRange(validationResult.Errors.ToValidationErrorList());

            return result;
        }

        public async Task<Order> GetAsync(Int32 orderId)
        {
            return _uow.Repository<Order>().Find(orderId);
        }
    }
}
