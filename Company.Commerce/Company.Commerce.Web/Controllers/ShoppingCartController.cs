using Company.Commerce.Entity.Models;
using Company.Commerce.Repository;
using Company.Commerce.Service;
using Company.Commerce.Service.Utility;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using Company.Commerce.Web.Utility;
using Company.Commerce.Web.ViewModels;
using Company.Commerce.Web.ViewModels.ShoppingCart;

namespace Company.Commerce.Web.Controllers
{
    public class ShoppingCartController : Controller
    {
        private const String KeyTempDataCartUpdates = "CartUpdates";

        private readonly IUnitOfWork _uow;
        private readonly IProductService _productService;
        private readonly IShoppingCartService _shoppingCartService;

        public ShoppingCartController(IUnitOfWork uow, IProductService productService, IShoppingCartService shoppingCartService)
        {
            _uow = uow;
            _productService = productService;
            _shoppingCartService = shoppingCartService;
        }

        //
        // GET: /ShoppingCart/
        public ActionResult Index()
        {
            IEnumerable<CartItem> cartItems =
                _shoppingCartService.GetCartWithProducts(Utility.Helpers.ShoppingCartHelpers.GetShoppingCartId(this.ControllerContext));

            //TODO refresh cart items/do updates with messages..no longer available etc

            ShoppingCartIndexViewModel model = new ShoppingCartIndexViewModel()
            {
                CartItems = cartItems.ToList()
            };

            Object cartUpdates;

            if (TempData.TryGetValue(KeyTempDataCartUpdates, out cartUpdates))
                model.CartUpdates.AddRange(cartUpdates as IEnumerable<String>);

            return View(model);
        }

        //
        // POST: /ShoppingCart/AddToCart
        public async Task<ActionResult> AddToCart(Int32 productId)
        {
            CartItem existing = _shoppingCartService.Get(Utility.Helpers.ShoppingCartHelpers.GetShoppingCartId(this.ControllerContext), productId);

            List<String> cartUpdates = new List<String>();

            if (existing != null)
            {
                existing.Quantity++;

                ServiceOperationResult result = await _shoppingCartService.UpdateAsync(existing);

                if (result.Succeeded)
                {
                    _uow.Commit();

                    cartUpdates.Add("The item was successfully added to your cart.");
                }
                else
                    cartUpdates.AddRange(result.Errors.Select(ve => ve.Error));
            }
            else
            {
                CartItem toCreate = new CartItem()
                {
                    ProductId = productId,
                    Quantity = 1,
                    ShoppingCartId = Utility.Helpers.ShoppingCartHelpers.GetShoppingCartId(this.ControllerContext)
                };

                ServiceOperationResult<CartItem> result = await _shoppingCartService.CreateAsync(toCreate);

                if (result.Succeeded)
                {
                    _uow.Commit();

                    cartUpdates.Add("The item was successfully added to your cart.");
                }
                else
                    cartUpdates.AddRange(result.Errors.Select(ve => ve.Error));
            }

            //If any errors were picked up along the way add them to temp data before redirect.
            TempData.Add(KeyTempDataCartUpdates, cartUpdates);

            return RedirectToAction("Index");
        }

        //
        // POST: /ShoppingCart/UpdateCartItem
        [HttpPost]
        public async Task<ActionResult> UpdateCartItemQuantity(Int32 cartItemId, Int32 quantity)
        {
            CartItem toUpdate = _shoppingCartService.Get(cartItemId);

            //Make sure that the cart item being updated belongs to the current ShoppingCartId
            if (toUpdate.ShoppingCartId != Utility.Helpers.ShoppingCartHelpers.GetShoppingCartId(this.ControllerContext))
                return RedirectToAction("Index");

            toUpdate.Quantity = quantity;

            ServiceOperationResult result =
                await _shoppingCartService.UpdateAsync(toUpdate);

            if (result.Succeeded)
            {
                _uow.Commit();

                TempData.Add(KeyTempDataCartUpdates, new List<String>() { "The item was successfully updated." });
            }
            else
                TempData.Add(KeyTempDataCartUpdates, result.Errors.Select(ve => ve.Error));

            return RedirectToAction("Index");
        }

        //
        // POST: /ShoppingCart/RemoveFromCart/
        [HttpPost]
        public ActionResult RemoveFromCart(Int32 cartItemId)
        {
            CartItem cartItem = _shoppingCartService.Get(cartItemId);

            if (cartItem == null || cartItem.ShoppingCartId != Utility.Helpers.ShoppingCartHelpers.GetShoppingCartId(this.ControllerContext))
                return RedirectToAction("Index");

            _shoppingCartService.Delete(cartItem);

            _uow.Commit();

            TempData.Add(KeyTempDataCartUpdates, new List<String>() { "The item has been removed from your cart." });

            return RedirectToAction("Index");
        }

        #region Child Actions
        [ChildActionOnly]
        public ActionResult _ShoppingCartModal()
        {
            IEnumerable<CartItem> cartItems =
                _shoppingCartService.GetCartWithProducts(Utility.Helpers.ShoppingCartHelpers.GetShoppingCartId(this.ControllerContext));

            ShoppingCartModalViewModel model = new ShoppingCartModalViewModel()
            {
                CartItems = cartItems
            };

            return PartialView("~/Views/Shared/_ShoppingCartModal.cshtml", model);
        }

        [ChildActionOnly]
        public ActionResult _ShoppingCartNavBar()
        {
            ViewBag.CartItemCount =
                _shoppingCartService.GetCart(Utility.Helpers.ShoppingCartHelpers.GetShoppingCartId(this.ControllerContext))
                .Sum(ci => ci.Quantity);

            return PartialView();
        }

        #endregion
    }
}