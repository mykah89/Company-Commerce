using Company.Commerce.Entity.Models;
using Company.Commerce.Service.Utility;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Company.Commerce.Service
{
    public interface IShoppingCartService
    {
        Task<ServiceOperationResult<CartItem>> CreateAsync(CartItem cartItem);

        void Delete(CartItem cartItem);

        CartItem Get(String shoppingCartId, Int32 productId);

        CartItem Get(Int32 cartItemId);

        IEnumerable<CartItem> GetCart(String shoppingCartId);

        IEnumerable<CartItem> GetCartWithProducts(String shoppingCartId);

        IEnumerable<CartItem> GetCartWithProductsAndImages(String shoppingCartId);

        Task<ServiceOperationResult> UpdateAsync(CartItem cartItem);
    }
}
