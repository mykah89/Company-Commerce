using AutoMapper;
using Framework.Repository;
using Project.MVC.Entity.Models;
using Project.MVC.Service;
using Project.MVC.Web.ViewModels.Checkout;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Caching;
using System.Web.Mvc;
using Microsoft.AspNet.Identity;
using Project.MVC.Web.ViewModels.General;
using Project.MVC.Web.ViewModels.Checkout.Steps;
using Project.MVC.Web.Filters;
using FluentValidation.Mvc;
using Project.MVC.Web.Features.Extensions;
using Project.MVC.Web.ViewModels.ShoppingCart;
using Project.MVC.Web.Helpers;
using System.Globalization;
using System.Transactions;
using Project.MVC.Service.Models;
using System.Data.Entity.Infrastructure;
using Project.MVC.Services.Email;
using System.Net.Mail;
using System.Text;

namespace Project.MVC.Web.Controllers
{
    [Authorize]
    public class CheckoutController : Controller
    {
        private IAddressService _addressService;
        private ICreditCardService _creditCardService;
        private IEmailService _emailService;
        private IOrderService _orderService;
        private IProductService _productService;
        private ITransactionService _transactionService;
        private ICartService _shoppingCartService;
        private IUnitOfWork _uow;
        private IUserService _userService;

        public CheckoutController(IUnitOfWork uow,
            IAddressService addressService,
            ICreditCardService creditCardService,
            IEmailService emailService,
            IOrderService orderService,
            IProductService productService,
            ITransactionService transactionService,
            ICartService shoppingCartService,
            IUserService userService)
        {
            _addressService = addressService;
            _creditCardService = creditCardService;
            _emailService = emailService;
            _orderService = orderService;
            _productService = productService;
            _shoppingCartService = shoppingCartService;
            _transactionService = transactionService;
            _uow = uow;
            _userService = userService;
        }

        //
        // GET: /Checkout/
        [AllowAnonymous]
        public ActionResult Index()
        {
            return View();
        }

        //UNDONE POST ONLY
        //
        // POST: /Checkout/Checkout
        //[ValidateAntiForgeryToken]
        public ActionResult Checkout()
        {
            //Get current cart items
            var currentCartItems = _shoppingCartService
                .GetCartWithAssociatedProducts(ShoppingCartHelpers.GetShoppingCartID(this.ControllerContext)).ToList();

            //If there are no cart items, redirect to the shoppingcart index
            if (!currentCartItems.Any())
                return RedirectToAction("Index", "ShoppingCart");

            //Construct the model that will be the container for the collected checkout information
            CheckoutViewModel checkoutModel = new CheckoutViewModel(currentCartItems, Guid.NewGuid().ToString("N"));

            //Cache the container model at a well known location unique to the user, sliding expiration of 10 minutes
            CacheHelpers.InsertCheckoutInstanceIntoCache(checkoutModel, this.ControllerContext);

            //Construct & hydrate the first step viewmodel
            var availableAddressesForUser = _userService.GetUserWithAddresses(IdentityHelpers.GetUserId(User.Identity)).Addresses.AsEnumerable();

            var model = checkoutModel.GetShippingViewModel(availableAddressesForUser);

            #region TestInformation
#if DEBUG
            model.Email = "mykah89@hotmail.com";
            model.PhoneNumber = "1234567890";
#endif
            #endregion

            return View("ShippingInformation", model);
        }

        //
        // POST: /Checkout/ShippingInformation
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult ShippingInformation(ShippingInformationViewModel model)
        {
            CheckoutViewModel checkoutModel;

            if (CacheHelpers.GetCheckoutInstanceFromCache(model.CheckoutInstance, this.ControllerContext, out checkoutModel) != CacheHelpers.CheckoutCacheStatus.Success)
                return RedirectToAction("Index", "ShoppingCart");

            if (ModelState.IsValid)
            {
                Address shippingAddress;

                //If the user selected an address
                if (model.SelectedShippingAddressId > 0)
                {
                    var address = _addressService.Get(model.SelectedShippingAddressId);

                    //Verify it exists
                    if (address != null)
                    {
                        //Verify it belongs to this user
                        if (address.UserID == IdentityHelpers.GetUserId(this.User.Identity))
                            shippingAddress = address;
                        else
                            throw new InvalidOperationException("Address does not belong to this user.");
                    }
                    else
                        throw new InvalidOperationException("Expected address, but not found.");
                }
                else
                    shippingAddress = new Address { AddressLine1 = model.AddressLine1, AddressLine2 = model.AddressLine2, City = model.City, PostalCode = model.PostalCode, State = model.State };

                checkoutModel.Email = model.Email;

                checkoutModel.PhoneNumber = model.PhoneNumber;

                checkoutModel.ShippingAddress = shippingAddress;

                checkoutModel.ShippingCost = _orderService.CalculateShipping(checkoutModel.CartItems);

                checkoutModel.Tax = _orderService.CalculateTax(checkoutModel.CartItems);

                var nextModel = checkoutModel.GetBillingViewModel();

                #region Test Information
#if DEBUG
                nextModel.CreditCardPayment = new CreditCardPaymentViewModel()
                {
                    //Authorize.Net Test Visa
                    CreditCardNumber = "4007000000027",
                    CVV = "123",
                    ExpirationMonth = 1,
                    ExpirationYear = 2018
                };

                nextModel.BillingSameAsShipping = true;
#endif
                #endregion

                ModelState.Clear();

                return View("BillingInformation", nextModel);
            }
            else
            {
                //Resupply unbound information the model needs to redisplay
                checkoutModel.GetShippingViewModel(_userService
                    .GetUserWithAddresses(IdentityHelpers.GetUserId(User.Identity)).Addresses);

                return View(model);
            }
        }

        //
        // POST: /Checkout/BillingInformation
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult BillingInformation(BillingInformationViewModel model)
        {
            CheckoutViewModel checkoutModel;

            if (CacheHelpers.GetCheckoutInstanceFromCache(model.CheckoutInstance, this.ControllerContext, out checkoutModel) != CacheHelpers.CheckoutCacheStatus.Success)
                return RedirectToAction("Index", "ShoppingCart");

            if (NextButtonWasClicked())
            {
                if (ModelState.IsValid)
                {
                    Address billingAddress;

                    if (model.BillingSameAsShipping)
                        billingAddress = checkoutModel.ShippingAddress;
                    else
                        billingAddress = new Address() { AddressLine1 = model.AddressLine1, AddressLine2 = model.AddressLine2, City = model.City, State = model.State, PostalCode = model.PostalCode };

                    checkoutModel.BillingAddress = billingAddress;

                    checkoutModel.CreditCardPayment = model.CreditCardPayment;

                    //checkoutModel.GiftCardPayments = model.GiftCardPayments;

                    var nextModel = checkoutModel.GetReviewViewModel();

                    return View("Review", nextModel);
                }
                else
                {
                    //Resupply unbound information the model needs to redisplay
                    model = checkoutModel.GetBillingViewModel();

                    //UNDONE
                    //Clear any modelstate entries for credit card to avoid posting them back
                    //ModelState.ToList().ForEach(kvp =>
                    //    {
                    //        if (kvp.Key.Contains("CreditCard"))
                    //        {
                    //            var emptyVal = new ValueProviderResult(String.Empty, String.Empty, CultureInfo.CurrentCulture);
                    //            ModelState.SetModelValue(kvp.Key, emptyVal);
                    //        }
                    //    });

                    return View(model);
                }
            }
            else if (PreviousButtonWasClicked())
            {
                var previousModel = checkoutModel.GetShippingViewModel(_userService
                    .GetUserWithAddresses(IdentityHelpers.GetUserId(User.Identity)).Addresses);

                this.ModelState.Clear();

                return View("ShippingInformation", previousModel);
            }

            throw new HttpException(500, "Invalid destination.");
        }

        //
        // POST: /Checkout/ReviewInformation
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Review(ReviewViewModel model)
        {
            CheckoutViewModel checkoutModel;

            if (CacheHelpers.GetCheckoutInstanceFromCache(model.CheckoutInstance, this.ControllerContext, out checkoutModel) != CacheHelpers.CheckoutCacheStatus.Success)
                return RedirectToAction("Index", "ShoppingCart");

            //Check to see where the user was trying to go
            if (ConfirmButtonWasClicked())
            {
                //Clear the checkoutmodel from cache to prevent reprocessing
                CacheHelpers.RemoveCheckoutInstanceFromCache(this.ControllerContext);

                Order toCreate = checkoutModel.GetOrder(IdentityHelpers.GetUserId(this.User.Identity));

                Boolean orderCreated = false;
                Int32 attempts = 3;

                Order newOrder = null;

                using (TransactionScope orderProcessingScope = new TransactionScope())
                {
                    do
                    {
                        try
                        {
                            using (TransactionScope createOrderScope = new TransactionScope())
                            {
                                _shoppingCartService.EmptyCart(ShoppingCartHelpers.GetShoppingCartID(this.ControllerContext));

                                if (!_productService.DecrementInventory(toCreate.OrderDetails))
                                    return RedirectToAction("StockUnavailable", "Checkout");

                                newOrder = _orderService.CreateOrder(toCreate);

                                _uow.Save();

                                orderCreated = true;

                                createOrderScope.Complete();
                            }
                        }
                        catch (DbUpdateConcurrencyException ex)
                        {
                            //Reload any entry that changed with new values before we retry
                            foreach (var item in ex.Entries)
                            {
                                item.Reload();
                            }

                            attempts--;
                        }

                    } while (!orderCreated && attempts > 0);

                    if (orderCreated)
                    {
                        using (TransactionScope scope = new TransactionScope())
                        {
                            var payment = new CreditCardPayment()
                            {
                                Amount = newOrder.SubTotal + newOrder.Tax + newOrder.ShippingCost,
                                CreditCardNumber = checkoutModel.CreditCardPayment.CreditCardNumber,
                                CVV = checkoutModel.CreditCardPayment.CVV,
                                Expiration = new DateTime(checkoutModel.CreditCardPayment.ExpirationYear, checkoutModel.CreditCardPayment.ExpirationMonth, DateTime.DaysInMonth(checkoutModel.CreditCardPayment.ExpirationYear, checkoutModel.CreditCardPayment.ExpirationMonth), 0, 0, 0)
                            };

                            //Authorize the payment
                            var paymentAuthResult = _creditCardService
                                .Authorize(payment, "Payment for order " + newOrder.OrderID, newOrder.BillingAddress);

                            //If successful authorization
                            if (paymentAuthResult.Success)
                            {
                                _transactionService.Create(paymentAuthResult.Transaction);

                                newOrder.OrderStatus = OrderStatus.Processing;

                                _orderService.Update(newOrder);

                                _uow.Save();

                                _emailService.SendMessage(EmailHelpers.GetOrderReceivedEmail(this, newOrder.Email, newOrder.OrderID));

                                scope.Complete();

                                orderProcessingScope.Complete();

                                TempData.Add("OrderID", newOrder.OrderID);

                                return RedirectToAction("Confirmation");
                            }
                            else
                            {
                                paymentAuthResult.Errors.ForEach(e => ModelState.AddModelError("", e));
                            }
                        }
                    }
                    else
                        ModelState.AddModelError("", "We're sorry, there was a problem creating your order. " +
                    "Please try again and if the problem persists, please try again later.");
                }

                CacheHelpers.InsertCheckoutInstanceIntoCache(checkoutModel, this.ControllerContext);

                var reModel = checkoutModel.GetReviewViewModel();

                return View("Review", reModel);
            }
            else if (PreviousButtonWasClicked())
            {
                var previousModel = checkoutModel.GetBillingViewModel();

                ModelState.Clear();

                return View("BillingInformation", previousModel);
            }

            throw new HttpException(500, "Invalid destination.");
        }

        //
        // GET: /Checkout/Confirmation
        public ActionResult Confirmation()
        {
            object tempOrderId;

            if (TempData.TryGetValue("OrderID", out tempOrderId))
            {
                var orderID = Convert.ToInt32(tempOrderId);
                //var order = _orderService.GetOrder(Convert.ToInt32(tempOrderId));

                //return View(Mapper.Map<OrderViewModel>(order));
                ViewBag.OrderID = tempOrderId;
            }

            return View();
        }

        //
        // GET: /Checkout/_GiftCardPaymentRow
        //[AjaxOnly]
        [AllowAnonymous]
        [AjaxOnly]
        public ActionResult _CheckoutGiftCardRow()
        {
            return PartialView("~/Views/Checkout/EditorTemplates/EditorGiftcardCheckout.cshtml", new GiftCardPaymentViewModel());
        }

        [AllowAnonymous]
        [AjaxOnly]
        [HttpPost]
        public ActionResult _AddressFixUp(Address address)
        {
            var result = new JsonDataResultViewModel();
            var model = new AddressFixUpViewmodel();

            model.ExistingAddress = address;

            var addressResult = _addressService.ValidateAddress(Mapper.Map<Address>(address));

            if (result != null && addressResult.Alternates.Any())
            {
                var suggAdd = addressResult.Alternates.First();

                suggAdd.State = Helpers.Converters.StateAbbrvToState(suggAdd.State);

                //TODO if suggested address is a direct match for the posted address, return false so
                //there is no fixup slide down.
                //if (suggAdd.Equals(address))
                //{
                //    result.Success = false;

                //    return Json(result);
                //}

                model.SuggestedAddress = suggAdd;

                result.Success = true;

                var viewResult = ViewHelpers
                    .RenderViewToString(this.ControllerContext,
                    "~/Views/Checkout/_AddressFixUp.cshtml",
                    model);

                result.Data = viewResult;
            }
            else
                result.Success = false;

            return Json(result);
        }

        #region Helpers
        private Boolean ConfirmButtonWasClicked()
        {
            return this.HttpContext.Request.Form["Confirm"] != null;
        }

        private Boolean NextButtonWasClicked()
        {
            return this.HttpContext.Request.Form["Next"] != null;
        }

        private Boolean PreviousButtonWasClicked()
        {
            return this.HttpContext.Request.Form["Previous"] != null;
        }
        #endregion
    }
}