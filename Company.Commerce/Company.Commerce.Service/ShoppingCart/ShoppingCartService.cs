using Company.Commerce.Entity.Models;
using Company.Commerce.Repository;
using Company.Commerce.Service.Utility;
using Company.Commerce.Service.Validation.FluentValidation;
using FluentValidation.Results;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FluentValidation;

namespace Company.Commerce.Service
{
    public class ShoppingCartService : IShoppingCartService
    {
        private readonly IProductService _productService;

        private readonly IUnitOfWork _uow;

        private readonly FluentCartItemValidator _cartItemValidator;

        public ShoppingCartService(IUnitOfWork uow, IProductService productService)
        {
            _productService = productService;

            _uow = uow;

            _cartItemValidator = new FluentCartItemValidator(productService);
        }

        public async Task<ServiceOperationResult<CartItem>> CreateAsync(CartItem cartItem)
        {
            if (cartItem == null)
                throw new ArgumentNullException("cartItem");

            ServiceOperationResult<CartItem> result = new ServiceOperationResult<CartItem>();

            ValidationResult validationResult = await _cartItemValidator.ValidateAsync(cartItem);

            if (validationResult.IsValid)
            {
                result.Data = _uow.Repository<CartItem>().Add(cartItem);

                result.Succeeded = true;
            }
            else
                result.Errors.AddRange(validationResult.Errors.ToValidationErrorList());

            return result;
        }

        public void Delete(CartItem cartItem)
        {
            if (cartItem == null)
                throw new ArgumentNullException("cartItem");

            _uow.Repository<CartItem>().Delete(cartItem);
        }

        public CartItem Get(String shoppingCartId, Int32 productId)
        {
            if (String.IsNullOrWhiteSpace(shoppingCartId))
                throw new ArgumentNullException("shoppingCartId");

            return _uow.Repository<CartItem>().Query()
                .Filter(ci => ci.ShoppingCartId == shoppingCartId && ci.ProductId == productId)
                .Get()
                .FirstOrDefault();
        }

        public CartItem Get(Int32 cartItemId)
        {
            return _uow.Repository<CartItem>().Find(cartItemId);
        }

        public async Task<ServiceOperationResult> UpdateAsync(CartItem cartItem)
        {
            if (cartItem == null)
                throw new ArgumentNullException("cartItem");

            ServiceOperationResult result = new ServiceOperationResult();

            ValidationResult validationResult = await _cartItemValidator.ValidateAsync(cartItem);

            if (validationResult.IsValid)
            {
                _uow.Repository<CartItem>().Update(cartItem);

                result.Succeeded = true;
            }
            else
                result.Errors.AddRange(validationResult.Errors.ToValidationErrorList());

            return result;
        }

        public IEnumerable<CartItem> GetCart(String shoppingCartId)
        {
            if (String.IsNullOrWhiteSpace(shoppingCartId))
                throw new ArgumentNullException("shoppingCartId");

            return _uow.Repository<CartItem>().Query()
                .Filter(ci => ci.ShoppingCartId == shoppingCartId)
                .Get();
        }

        public IEnumerable<CartItem> GetCartWithProducts(String shoppingCartId)
        {
            if (String.IsNullOrWhiteSpace(shoppingCartId))
                throw new ArgumentNullException("shoppingCartId");

            return _uow.Repository<CartItem>().Query()
                .Filter(ci => ci.ShoppingCartId == shoppingCartId)
                .Include(ci => ci.Product)
                .Get();
        }

        public IEnumerable<CartItem> GetCartWithProductsAndImages(String shoppingCartId)
        {
            if (String.IsNullOrWhiteSpace(shoppingCartId))
                throw new ArgumentNullException("shoppingCartId");

            return _uow.Repository<CartItem>()
                .Query()
                .Filter(ci => ci.ShoppingCartId == shoppingCartId)
                .Include(ci => ci.Product)
                .Include(ci => ci.Product.AssociatedImages.Select(ai => ai.Image))
                .Get();
        }
    }
}
