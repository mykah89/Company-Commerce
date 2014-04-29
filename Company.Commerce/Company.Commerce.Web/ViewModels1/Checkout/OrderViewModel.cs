using Project.MVC.Entity.Models;
using Project.MVC.Web.ViewModels.General;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Project.MVC.Web.ViewModels.Checkout
{
    //[Bind(Exclude = "TemporaryId, TotalPrice")]
    //public class OrderViewModel
    //{
    //    public OrderViewModel()
    //    {
    //        BillingAddress = new AddressViewModel();
    //        OrderDetails = new List<OrderDetailViewModel>();
    //        PendingGiftCardPayments = new List<GiftCardPaymentViewModel>();
    //        ShippingAddress = new AddressViewModel();
    //        Transactions = new List<TransactionViewModel>();
    //    }

    //    public Int32 OrderID { get; set; }

    //    public decimal BasePrice { get; set; }

    //    public Int32 BillingAddressID { get; set; }

    //    public String Email { get; set; }

    //    public DateTime OrderDate { get; set; }

    //    public OrderStatus OrderStatus { get; set; }

    //    public String PhoneNumber { get; set; }

    //    public Int32 ShippingAddressID { get; set; }

    //    public decimal ShippingCost { get; set; }

    //    public DateTime? ShippedDate { get; set; }

    //    public decimal Tax { get; set; }

    //    public decimal TotalPrice
    //    {
    //        get
    //        {
    //            return (this.BasePrice + this.ShippingCost + this.Tax);
    //        }
    //    }

    //    public Int32? UserID { get; set; }

    //    public virtual AddressViewModel BillingAddress { get; set; }
    //    public virtual ICollection<OrderDetailViewModel> OrderDetails { get; set; }

    //    public virtual CreditCardPaymentViewModel PendingCreditCardPayment { get; set; }
    //    public virtual List<GiftCardPaymentViewModel> PendingGiftCardPayments { get; set; }

    //    public virtual AddressViewModel ShippingAddress { get; set; }
    //    public virtual ICollection<TransactionViewModel> Transactions { get; set; }
    //    //public virtual UserViewModel User { get; set; }
    //}
}