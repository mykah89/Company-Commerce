using AutoMapper;
using FluentValidation.Mvc;
using Framework.Repository;
using Project.MVC.Entity.Models;
using Project.MVC.Service;
using Project.MVC.Web.Binders.ShoppingCart;
using Project.MVC.Web.Features;
using Project.MVC.Web.Filters;
using Project.MVC.Web.Helpers;
using Project.MVC.Web.ViewModels.General;
using Project.MVC.Web.ViewModels.ShoppingCart;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Web;
using System.Web.Mvc;

namespace Project.MVC.Web.Controllers
{
    public class ShoppingCartController : Controller
    {
        private String _shoppingCartID { get { return ShoppingCartHelpers.GetShoppingCartID(this.ControllerContext); } }
        private ICartService _cartService;
        private IProductService _productService;
        private IUnitOfWork _uow;
        private IUserService _userService;

        public ShoppingCartController(IUnitOfWork uow, ICartService shoppingCartService, IProductService productService, IUserService userService)
        {
            _cartService = shoppingCartService;
            _productService = productService;
            _uow = uow;
            _userService = userService;
        }

        //
        // GET: /ShoppingCart/
        public ActionResult Index()
        {
            object importantCartUpdates;
            TempData.TryGetValue("ImportantCartUpdates", out importantCartUpdates);

            object existingModelState;
            if (TempData.TryGetValue("ModelState", out existingModelState))
                this.ModelState.Merge((ModelStateDictionary)existingModelState);

            var model = new ShoppingCartIndexViewModel();

            model.CartItems = _cartService.GetCartWithAssociatedProducts(this._shoppingCartID).ToList();
            model.ImportantCartUpdates = importantCartUpdates as List<String> ?? new List<String>();
            model.ShoppingCartTotalPrice = model.CartItems.Sum(ci => ci.Quantity * ci.Product.UnitPrice);

            return View(model);
        }

        //
        // POST: /ShoppingCart/Index
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Index(ShoppingCartIndexViewModel model)
        {
            model.CartItems = _cartService.GetCartWithAssociatedProducts(this._shoppingCartID).ToList();

            if (!String.IsNullOrEmpty(model.PostalCode))
            {
                //TODO calculate actual shipping
                //modelShippingCost = _shippingService.CalculateShipping(model.CartItems);
                model.ShippingCost = 5.00m;
            }

            if (model.PromoCodes.Any())
            {
                //
            }

            model.ShoppingCartTotalPrice = model.CartItems.Sum(ci => ci.Quantity * ci.Product.UnitPrice) + model.ShippingCost;

            return View(model);
        }

        //
        // POST:/ShoppingCart/AddToCart/
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult AddToCart(Int32 productID, Int32 quantity = 1)
        {
            var product = _productService.Get(productID);

            if (product == null)
                return new HttpNotFoundResult("Product could not be found.");

            var user = _userService.Get(IdentityHelpers.GetUserId(this.User.Identity));

            var cartItem = _cartService.GetCart(user).FirstOrDefault(ci => ci.ProductID == productID);

            if (cartItem != null)
            {
                
            }

            List<String> cartUpdates = _cartService.AddItemToCart(product, user, quantity);

            if (!cartUpdates.Any())
            {
                _uow.Save();
            }

            TempData.Add("ImportantCartUpdates", cartUpdates);

            return RedirectToAction("Index");
        }

        //
        // POST: /ShoppingCart/RemoveFromCart
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult RemoveFromCart(Int32 cartItemID)
        {
            var importantCartUpdates = new List<String>();

            var user = _userService.Get(IdentityHelpers.GetUserId(this.User.Identity));

            var cartItems = _cartService.GetCart(user);

            if (cartItems.FirstOrDefault(ci => ci.CartItemID == cartItemID) != null)
            {
                _cartService.RemoveItemFromCart(user, cartItemID);

                _uow.Save();

                importantCartUpdates.Add("The item has been removed from your cart.");
            }
            else
                return new HttpUnauthorizedResult();

            TempData.Add("ImportantCartUpdates", importantCartUpdates);

            return RedirectToAction("Index");
        }

        //
        // POST: /ShoppingCart/UpdateCartItemQuantity
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult UpdateCartItemQuantity(UpdateCartItemQuantityViewModel model)
        {
            var importantCartUpdates = new List<String>();

            if (ModelState.IsValid)
            {
                var existingItem = _cartService.GetItem(model.CartItemID);

                if (existingItem == null)
                    //return HttpNotFound("The cart item to update could not be found.");
                    throw new HttpException(500, "The cart item to update could not be found.");
                //return new HttpStatusCodeResult(System.Net.HttpStatusCode.NotFound, "The cart item to update could not be found.");

                if (existingItem.ShoppingCartID != this._shoppingCartID)
                    throw new HttpException(403, "The cart item does not belong to the current user.");
                //return new HttpStatusCodeResult(System.Net.HttpStatusCode.Unauthorized, "The cart item does not belong to the current user.");

                if (model.Quantity <= 0)
                {
                    _cartService.Delete(existingItem);

                    importantCartUpdates.Add("The item has been successfully removed from your cart.");
                }
                else
                {
                    var product = _productService.Get(existingItem.ProductID);

                    if (model.Quantity >= product.UnitsInStock)
                    {
                        existingItem.Quantity = product.UnitsInStock;

                        _cartService.Update(existingItem);

                        importantCartUpdates
                            .Add(String.Format("You requested more than the available amount for {0}. " +
                            "The item has been updated to reflect the available quantity of ({1}).", product.ProductName, product.UnitsInStock));
                    }
                    else
                    {
                        existingItem.Quantity = model.Quantity;

                        _cartService.Update(existingItem);

                        importantCartUpdates
                            .Add("The item has been successfully updated.");
                    }
                }

                _uow.Save();

            }
            else
            {
                TempData.Add("ModelState", this.allErrorsToModelErrors(this.ModelState));
            }

            TempData.Add("ImportantCartUpdates", importantCartUpdates);

            return RedirectToAction("Index");

            #region Ajax
            ////if (Request.IsAjaxRequest())
            ////{
            ////    var jsonResult = new JsonDataResultViewModel();

            ////    jsonResult.Success = true;

            ////    return Json(jsonResult);
            ////}
            #endregion
        }

        [ChildActionOnly]
        public ActionResult _ShoppingCartModal()
        {
            var model = new ShoppingCartModalViewModel()
                {
                    CartItems = _cartService.GetCartWithAssociatedProducts(_shoppingCartID)
                };

            return PartialView(model);
        }

        [ChildActionOnly]
        public ActionResult _ShoppingCartNavBar()
        {
            ViewBag.CartItemCount = _cartService.GetCart(_shoppingCartID).Count();

            return PartialView();
        }

        #region Helpers
        private ModelStateDictionary allErrorsToModelErrors(ModelStateDictionary msd)
        {
            var newModelState = new ModelStateDictionary();

            this.ModelState.ToList().ForEach(mse =>
            {
                if (mse.Value.Errors.Any())
                {
                    foreach (var item in mse.Value.Errors)
                    {
                        newModelState.AddModelError("", item.ErrorMessage);
                    }
                }
            });

            return newModelState;
        }
        #endregion

    }
}