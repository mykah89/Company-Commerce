using Project.MVC.Web.ViewModels.Checkout;
using Project.MVC.Web.ViewModels.Store;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Project.MVC.Web.ViewModels.Checkout
{
    public class OrderDetailViewModel
    {
        public Int32 OrderDetailID { get; set; }
        public Int32 OrderID { get; set; }
        public Int32 ProductID { get; set; }

        public float Discount { get; set; }
        public Int32 Quantity { get; set; }
        public decimal UnitPrice { get; set; }

        public virtual ProductViewModel Product { get; set; }
    }
}