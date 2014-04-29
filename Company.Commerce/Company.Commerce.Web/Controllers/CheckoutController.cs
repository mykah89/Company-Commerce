using Company.Commerce.Entity.Models;
using Company.Commerce.Repository;
using Company.Commerce.Service;
using Company.Commerce.Service.Utility;
using Company.Commerce.Web.Helpers;
using Company.Commerce.Web.Settings;
using Company.Commerce.Web.ViewModels.Checkout;
using Company.Commerce.Web.ViewModels.Shared;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Caching;
using System.Threading.Tasks;
using System.Transactions;
using System.Web;
using System.Web.Caching;
using System.Web.Mvc;

namespace Company.Commerce.Web.Controllers
{
    public class CheckoutController : Controller
    {
        private const String KeyCheckoutCacheLocation = "CheckoutCache-";

        private readonly IOrderService _orderService;

        private readonly IShippingService _shippingService;

        private readonly IShoppingCartService _shoppingCartService;

        private readonly IUnitOfWork _uow;

        private readonly IUserService _userService;

        public CheckoutController(IUnitOfWork uow, IOrderService orderService, IShippingService shippingService, IShoppingCartService shoppingCartService, IUserService userService)
        {
            _orderService = orderService;
            _shippingService = shippingService;
            _shoppingCartService = shoppingCartService;
            _uow = uow;
            _userService = userService;
        }

        //
        // GET: /Checkout/
        public ActionResult Index()
        {
            if (!CheckoutSettings.CheckoutIsEnabled)
                return RedirectToAction("Index", "Home");

            String uniqueCacheKey = KeyCheckoutCacheLocation + Guid.NewGuid().ToString("N");

            CheckoutViewModel checkoutInstance = prepareCheckoutViewModel(uniqueCacheKey);

            //If there are no items to checkout
            if (!checkoutInstance.OrderLines.Any())
                return RedirectToAction("Index", "Home");

            CacheItem item = new CacheItem(uniqueCacheKey, checkoutInstance);

            MemoryCache.Default.Add(item, CheckoutSettings.CheckoutInstanceCachePolicy);

            ShippingInformationViewModel model = prepareShippingInformationViewModel(checkoutInstance);

            return View("ShippingInformation", model);
        }

        //
        // POST: /Checkout/ShippingInformation
        //[ValidateAntiForgeryToken]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult ShippingInformation(ShippingInformationViewModel model)
        {
            CacheItem cachedCheckoutInstance =
                MemoryCache.Default.GetCacheItem(model.InstanceKey);

            if (cachedCheckoutInstance != null)
            {
                CheckoutViewModel checkoutInstance = cachedCheckoutInstance.Value as CheckoutViewModel;

                if (checkoutInstance.InstanceKey != model.InstanceKey)
                    throw new InvalidOperationException("Checkout key mismatch.");

                if (nextButtonWasClicked())
                {
                    if (ModelState.IsValid)
                    {
                        applyShippingInformationToCheckoutInstance(checkoutInstance, model);

                        BillingInformationViewModel nextModel = prepareBillingInformationViewModel(checkoutInstance);

                        return View("BillingInformation", nextModel);
                    }

                    //Something failed

                    //Repopulate non bound items
                    model.OrderLines = checkoutInstance.OrderLines;
                    model.SalesTax = checkoutInstance.SalesTax;

                    return View(model);
                }

                throw new InvalidOperationException("Unexpected navigation.");
            }

            return Content("CheckoutExpired");
        }

        //
        // POST: /Checkout/BillingInformation
        //[ValidateAntiForgeryToken]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult BillingInformation(BillingInformationViewModel model)
        {
            CacheItem cachedCheckoutInstance =
                MemoryCache.Default.GetCacheItem(model.InstanceKey);

            if (cachedCheckoutInstance != null)
            {
                CheckoutViewModel checkoutInstance = cachedCheckoutInstance.Value as CheckoutViewModel;

                if (checkoutInstance.InstanceKey != model.InstanceKey)
                    throw new InvalidOperationException("Checkout key mismatch.");

                if (nextButtonWasClicked())
                {
                    if (ModelState.IsValid)
                    {
                        applyBillinginformationToCheckoutInstance(checkoutInstance, model);

                        ReviewViewModel nextModel = prepareReviewViewModel(checkoutInstance);

                        return View("Review", nextModel);
                    }


                    //Something failed

                    //Repopulate non-bound items
                    model.OrderLines = checkoutInstance.OrderLines;
                    model.SalesTax = checkoutInstance.SalesTax;
                    model.ShippingAddress = checkoutInstance.ShippingAddress;
                    model.ShippingCost = checkoutInstance.ShippingCost;

                    return View(model);
                }
                else if (previousButtonWasClicked())
                {
                    ModelState.Clear();

                    ShippingInformationViewModel previousModel = prepareShippingInformationViewModel(checkoutInstance);

                    return View("ShippingInformation", previousModel);
                }
                else
                    throw new InvalidOperationException("Unexpected navigation.");
            }

            return Content("CheckoutExpired");
        }

        //
        // POST: /Checkout/Review
        //[ValidateAntiForgeryToken]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Review(ReviewViewModel model)
        {
            CacheItem cachedCheckoutInstance =
                MemoryCache.Default.GetCacheItem(model.InstanceKey);

            if (cachedCheckoutInstance != null)
            {
                CheckoutViewModel checkoutInstance = cachedCheckoutInstance.Value as CheckoutViewModel;

                if (checkoutInstance.InstanceKey != model.InstanceKey)
                    throw new InvalidOperationException("Checkout key mismatch.");

                if (confirmButtonWasClicked())
                {
                    if (ModelState.IsValid)
                    {
                        //To prevent double processing of this order remove it from the cache
                        MemoryCache.Default.Remove(checkoutInstance.InstanceKey);


                        //TODO Cleanup + transaction processing with authorize.net

                        //_apiLoginID = "5hVHQz84B";
                        //_transactionKey = "653Dh9rH3W88juYU";

                        using (TransactionScope scope = new TransactionScope())
                        {
                            ServiceOperationResult<Order> result = await createOrderAsync(checkoutInstance);

                            if (result.Succeeded)
                            {
                                User currentUser = await _userService.GetAsync(Utility.Helpers.IdentityHelpers.GetUserId(this.User.Identity));

                                await _userService.SendEmailAsync(currentUser, EmailHelpers.UserEmails.OrderReceived(result.Data.OrderId));

                                try
                                {
                                    _uow.Commit();

                                    scope.Complete();

                                    return RedirectToAction("OrderSuccess");
                                }
                                catch (Exception ex)
                                {
                                    ModelState.AddModelError("", "There was a problem processing your order, if the problem persists, please try again later.");

                                    //TODO Logger elmah/notifications
                                }
                            }
                        }
                    }

                    //If we got this far something failed, reprepare the viewmodel and cache the checkout instance
                    CacheItem item = new CacheItem(checkoutInstance.InstanceKey, checkoutInstance);

                    MemoryCache.Default.Add(item, CheckoutSettings.CheckoutInstanceCachePolicy);

                    ReviewViewModel repreparedModel = prepareReviewViewModel(checkoutInstance);

                    return View(repreparedModel);
                }
                else if (previousButtonWasClicked())
                {
                    ModelState.Clear();

                    BillingInformationViewModel previousModel = prepareBillingInformationViewModel(checkoutInstance);

                    return View("BillingInformation", previousModel);
                }
            }

            return Content("CheckoutExpired");
        }

        //
        // GET: /Checkout/OrderSuccess
        public ActionResult OrderSuccess()//Int32 orderId)
        {
            return Content("Success");
        }

        #region OrderProcessing
        private async Task<ServiceOperationResult<Order>> createOrderAsync(CheckoutViewModel checkoutInstance)
        {
            User currentUser = await _userService.GetAsync(Utility.Helpers.IdentityHelpers.GetUserId(this.User.Identity));

            Order newOrder = new Order()
            {
                BasePrice = checkoutInstance.OrderLines.Sum(ol => ol.Price * ol.Quantity),
                OrderDate = DateTime.Now,
                OrderLines = checkoutInstance.OrderLines.ToList(),
                OrderStatus = OrderStatus.Created,
                ShippingCost = 4.00m,
                Tax = checkoutInstance.OrderLines.Sum(ol => ol.Price * ol.Quantity) * .0875m,
                UserId = currentUser.UserId
            };

            return await _orderService.CreateAsync(newOrder);
        }

        #endregion

        #region Helpers

        private void applyBillinginformationToCheckoutInstance(CheckoutViewModel checkoutInstance, BillingInformationViewModel model)
        {
            if (checkoutInstance == null)
                throw new ArgumentNullException("checkoutInstance");

            if (model == null)
                throw new ArgumentNullException("model");

            if (checkoutInstance.InstanceKey != model.InstanceKey)
                throw new InvalidOperationException("Checkout key mismatch.");

            if (model.BillingSameAsShipping)
            {
                checkoutInstance.BillingAddress = checkoutInstance.ShippingAddress;
            }
            else
            {
                if (checkoutInstance.BillingAddress == null)
                    checkoutInstance.BillingAddress = new AddressViewModel();

                checkoutInstance.BillingAddress.AddressLine1 = model.BillingAddress.AddressLine1.Trim();
                checkoutInstance.BillingAddress.AddressLine2 = model.BillingAddress.AddressLine2.Trim();
                checkoutInstance.BillingAddress.City = model.BillingAddress.City.Trim();
                checkoutInstance.BillingAddress.State = model.BillingAddress.State;
                checkoutInstance.BillingAddress.PostalCode = model.BillingAddress.PostalCode.Trim();
            }

            if (checkoutInstance.CreditCard == null)
                checkoutInstance.CreditCard = new CreditCardViewModel();

            checkoutInstance.CreditCard.CardNumber = model.CreditCard.CardNumber.Trim();
            checkoutInstance.CreditCard.ExpirationMonth = model.CreditCard.ExpirationMonth;
            checkoutInstance.CreditCard.ExpirationYear = model.CreditCard.ExpirationYear;
            checkoutInstance.CreditCard.VerificationNumber = model.CreditCard.VerificationNumber.Trim();
        }

        private void applyShippingInformationToCheckoutInstance(CheckoutViewModel checkoutInstance, ShippingInformationViewModel model)
        {
            if (checkoutInstance == null)
                throw new ArgumentNullException("checkoutInstance");

            if (model == null)
                throw new ArgumentNullException("model");

            if (checkoutInstance.InstanceKey != model.InstanceKey)
                throw new InvalidOperationException("Checkout key mismatch.");

            checkoutInstance.EmailAddress = model.EmailAddress.Trim();

            checkoutInstance.PhoneNumber = model.PhoneNumber.Trim();

            if (checkoutInstance.ShippingAddress == null)
                checkoutInstance.ShippingAddress = new AddressViewModel();

            checkoutInstance.ShippingAddress.AddressLine1 = model.ShippingAddress.AddressLine1.Trim();
            checkoutInstance.ShippingAddress.AddressLine2 = model.ShippingAddress.AddressLine2.Trim();
            checkoutInstance.ShippingAddress.City = model.ShippingAddress.City.Trim();
            checkoutInstance.ShippingAddress.State = model.ShippingAddress.State;
            checkoutInstance.ShippingAddress.PostalCode = model.ShippingAddress.PostalCode.Trim();

            checkoutInstance.ShippingCost = _shippingService.CalculateShipping(checkoutInstance.OrderLines);
        }

        private BillingInformationViewModel prepareBillingInformationViewModel(CheckoutViewModel model)
        {
            if (model == null)
                throw new ArgumentNullException("model");

            BillingInformationViewModel result = new BillingInformationViewModel();

            //If the billing address already exists this call is the result of a previous navigation request.
            if (model.BillingAddress != null)
            {
                //result.CreditCard = model.CreditCard;
                result.CreditCard = new CreditCardViewModel();

                result.BillingSameAsShipping = model.ShippingAddress == model.BillingAddress;

                if (!result.BillingSameAsShipping)
                    result.BillingAddress = model.BillingAddress;
            }

#if DEBUG
            if (model.BillingAddress == null)
            {
                result.BillingAddress = new AddressViewModel()
                {
                    AddressLine1 = "123 Address st.",
                    AddressLine2 = "apt 2",
                    City = "SomeCity",
                    State = StateCode.NY,
                    PostalCode = "12345"
                };

                result.CreditCard = new CreditCardViewModel()
                {
                    CardNumber = "0000000000000000",
                    ExpirationMonth = (Month)DateTime.Now.Month,
                    ExpirationYear = DateTime.Now.AddYears(0),
                    VerificationNumber = "123"
                };
            }
#endif

            result.InstanceKey = model.InstanceKey;

            result.OrderLines = model.OrderLines;

            result.SalesTax = model.SalesTax;

            result.ShippingAddress = model.ShippingAddress;

            result.ShippingCost = model.ShippingCost;

            return result;
        }

        private CheckoutViewModel prepareCheckoutViewModel(String instanceKey)
        {
            if (String.IsNullOrWhiteSpace(instanceKey))
                throw new ArgumentNullException("instanceId");

            CheckoutViewModel result = new CheckoutViewModel();

            IEnumerable<CartItem> cartItems = _shoppingCartService.GetCartWithProductsAndImages(Utility.Helpers.ShoppingCartHelpers.GetShoppingCartId(this.ControllerContext));

            result.InstanceKey = instanceKey;

            foreach (var cartItem in cartItems)
            {
                OrderLine lineFromCartItem = new OrderLine()
                {
                    Price = cartItem.Quantity * cartItem.Product.Price,
                    Product = cartItem.Product,
                    ProductId = cartItem.Product.ProductId,
                    Quantity = cartItem.Quantity
                };

                result.OrderLines.Add(lineFromCartItem);
            }

#if DEBUG
            if (result.OrderLines == null)
            {
                result.OrderLines = new List<OrderLine>()
                    {
                        new OrderLine()
                        {
                            Price = 4.00m,
                            ProductId = 1,
                            Quantity = 1
                        }
                    };
            }
#endif

            result.SalesTax = Math.Round(result.OrderLines.Sum(ol => ol.Price) * CheckoutSettings.SalesTaxRate, 2, MidpointRounding.AwayFromZero);

            return result;
        }

        private ReviewViewModel prepareReviewViewModel(CheckoutViewModel model)
        {
            if (model == null)
                throw new ArgumentNullException("model");

            ReviewViewModel result = new ReviewViewModel();

            result.BillingAddress = model.BillingAddress;

            result.CreditCard = new CreditCardReviewViewModel()
            {
                ExpirationMonth = model.CreditCard.ExpirationMonth.Value,
                ExpirationYear = model.CreditCard.ExpirationYear.Value.Year.ToString(),
                MaskedCardNumber = model.CreditCard.CardNumber.Substring(model.CreditCard.CardNumber.Length - 4),
                MaskedCVV = new String(model.CreditCard.VerificationNumber.Select(c => '*').ToArray())
            };

            result.EmailAddress = model.EmailAddress;

            result.InstanceKey = model.InstanceKey;

            result.OrderLines = model.OrderLines;

            result.PhoneNumber = model.PhoneNumber;

            result.SalesTax = model.SalesTax;

            result.ShippingAddress = model.ShippingAddress;

            result.ShippingCost = model.ShippingCost;

            return result;
        }

        private ShippingInformationViewModel prepareShippingInformationViewModel(CheckoutViewModel model)
        {
            if (model == null)
                throw new ArgumentNullException("model");

            ShippingInformationViewModel result = new ShippingInformationViewModel();

            result.OrderLines = model.OrderLines;

            if (model.ShippingAddress != null)
            {
                result.EmailAddress = model.EmailAddress;

                result.PhoneNumber = model.PhoneNumber;

                result.ShippingAddress = model.ShippingAddress;
            }
#if DEBUG
            if (model.ShippingAddress == null)
            {
                result.EmailAddress = "123@domain.com";

                result.PhoneNumber = "123-456-7890";

                result.ShippingAddress = new AddressViewModel()
                {
                    AddressLine1 = "123 Address st.",
                    AddressLine2 = "apt 2",
                    City = "SomeCity",
                    State = StateCode.NY,
                    PostalCode = "12345"
                };
            }
#endif
            result.InstanceKey = model.InstanceKey;

            result.SalesTax = model.SalesTax;

            return result;
        }

        #endregion

        #region Navigation Helpers
        private Boolean previousButtonWasClicked()
        {
            return Request.Form["Previous"] != null;
        }

        private Boolean nextButtonWasClicked()
        {
            return Request.Form["Next"] != null;
        }

        private Boolean confirmButtonWasClicked()
        {
            return Request.Form["Confirm"] != null;
        }
        #endregion
    }
}