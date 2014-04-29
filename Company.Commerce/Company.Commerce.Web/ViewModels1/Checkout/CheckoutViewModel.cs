using AutoMapper;
using Company.Commerce.Entity.Models;
using Project.MVC.Entity.Models;
using Project.MVC.Service;
using Project.MVC.Web.ViewModels.Checkout.Steps;
using Project.MVC.Web.ViewModels.ShoppingCart;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Project.MVC.Web.ViewModels.Checkout
{
    public class CheckoutViewModel
    {
        public CheckoutViewModel(IEnumerable<CartItem> cartItems, String checkoutInstance)
        {
            CartItems = cartItems;
            CheckoutInstance = checkoutInstance;
        }

        public Address BillingAddress { get; set; }

        public CreditCardPaymentViewModel CreditCardPayment { get; set; }

        public String Email { get; set; }

        public String PhoneNumber { get; set; }

        public IEnumerable<CartItem> CartItems { get; private set; }

        public String CheckoutInstance { get; private set; }

        public Order GetOrder(Int32 userID)
        {
            var result = new Order();

            result.BillingAddress = this.BillingAddress;
            result.BillingAddress.UserID = userID;

            result.Email = this.Email;

            result.OrderDate = DateTime.Now;

            result.OrderDetails = this.CartItems
                .Select(ci => new OrderDetail()
                {
                    ProductID = ci.ProductID,
                    Quantity = ci.Quantity,
                    UnitPrice = ci.Product.UnitPrice
                }).ToList();

            result.OrderStatus = OrderStatus.Created;

            result.PhoneNumber = this.PhoneNumber;

            result.ShippingAddress = this.ShippingAddress;
            result.ShippingAddress.UserID = userID;

            result.ShippingCost = this.ShippingCost;

            result.Tax = this.Tax;

            result.UserID = userID;

            return result;
        }

        //public List<GiftCardPaymentViewModel> GiftCardPayments { get; set; }

        public Address ShippingAddress { get; set; }

        public Decimal ShippingCost { get; set; }

        public Decimal Tax { get; set; }

        public BillingInformationViewModel GetBillingViewModel()
        {
            if (this.ShippingAddress == null)
                throw new InvalidOperationException("Shipping address must exist to create billing view model.");

            var result = new BillingInformationViewModel();

            //If the billing address exists, we are going forward from a previous or coming back from a next.
            if (this.BillingAddress != null)
            {
                if (BillingAddress.AddressID > 0)
                {
                    result.BillingSameAsShipping = this.ShippingAddress.AddressID == this.BillingAddress.AddressID;
                }
                else
                {
                    Boolean addressesAreSame = this.ShippingAddress.AddressLine1 == this.BillingAddress.AddressLine1
                    && this.ShippingAddress.AddressLine2 == this.BillingAddress.AddressLine2
                    && this.ShippingAddress.City == this.BillingAddress.City
                    && this.ShippingAddress.State == this.BillingAddress.State
                    && this.ShippingAddress.PostalCode == this.BillingAddress.PostalCode;

                    if (!addressesAreSame)
                    {
                        result.AddressLine1 = this.BillingAddress.AddressLine1;
                        result.AddressLine2 = this.BillingAddress.AddressLine2;
                        result.City = this.BillingAddress.City;
                        result.State = this.BillingAddress.State;
                        result.PostalCode = this.BillingAddress.PostalCode;
                    }

                    result.BillingSameAsShipping = addressesAreSame;
                }
            }

            result.CartItems = this.CartItems;

            result.CheckoutInstance = this.CheckoutInstance;

            //UNDONE
            //result.CreditCardPayment = this.CreditCardPayment;

            //UNDONE
            //result.GiftCardPayments = this.GiftCardPayments;

            result.ShippingAddress = this.ShippingAddress;

            result.ShippingCost = this.ShippingCost;

            result.Tax = this.Tax;

            return result;
        }

        public ShippingInformationViewModel GetShippingViewModel(IEnumerable<Address> availableAddresses)
        {
            var result = new ShippingInformationViewModel();

            if (availableAddresses != null && availableAddresses.Any())
                result.AvailableAddresses = availableAddresses;

            if (this.ShippingAddress != null)
            {
                //If the shipping address exists, we should also have these values.
                if (String.IsNullOrEmpty(this.Email))
                    throw new InvalidOperationException("Null value for email unexpected.");

                if (String.IsNullOrEmpty(this.PhoneNumber))
                    throw new InvalidOperationException("Null value for phone number unexpected.");

                //If the address was the result of a selected address the id will have a value, so we dont need to populate
                //view model fields
                if (this.ShippingAddress.AddressID == 0)
                {
                    result.AddressLine1 = this.ShippingAddress.AddressLine1;
                    result.AddressLine2 = this.ShippingAddress.AddressLine2;
                    result.City = this.ShippingAddress.City;
                    result.State = this.ShippingAddress.State;
                    result.PostalCode = this.ShippingAddress.PostalCode;
                }

                result.Email = this.Email;
                result.PhoneNumber = this.PhoneNumber;
            }

            result.CartItems = this.CartItems;

            result.CheckoutInstance = this.CheckoutInstance;

            return result;
        }

        public ReviewViewModel GetReviewViewModel()
        {
            if (this.BillingAddress == null)
                throw new InvalidOperationException("Cannot create review model without billing address.");

            if (this.ShippingAddress == null)
                throw new InvalidOperationException("Cannot create review model without shipping address.");

            var result = new ReviewViewModel();

            result.Order = this.GetOrder(0);

            result.CartItems = this.CartItems;

            result.CheckoutInstance = this.CheckoutInstance;

            //UNDONE
            //result.CreditCardPayment = this.CreditCardPayment;

            result.CreditCardPayment = new CreditCardPaymentViewModel();

            result.CreditCardPayment.CreditCardNumber = "Card Ending in " + this.CreditCardPayment.CreditCardNumber.Substring(this.CreditCardPayment.CreditCardNumber.Length - 4);
            result.CreditCardPayment.ExpirationMonth = this.CreditCardPayment.ExpirationMonth;
            result.CreditCardPayment.ExpirationYear = this.CreditCardPayment.ExpirationYear;
            result.CreditCardPayment.CVV = this.CreditCardPayment.CVV;

            //result.GiftCardPayments = this.GiftCardPayments;
            //result.GiftCardPayments = ??

            return result;
        }
    }
}