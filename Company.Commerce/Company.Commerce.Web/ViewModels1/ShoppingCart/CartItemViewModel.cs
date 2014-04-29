using Project.MVC.Web.Binders.ShoppingCart;
using Project.MVC.Web.ViewModels.Store;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Project.MVC.Web.ViewModels.ShoppingCart
{
    [Bind(Exclude = "ShoppingCartID")]
    [ModelBinder(typeof(BindCurrentShoppingCartIDToCartItem))]
    public class CartItemViewModel
    {
        public String ShoppingCartID { get; set; }

        public Int32 CartItemID { get; set; }

        public Int32 ProductID { get; set; }

        public Int32 Quantity { get; set; }

        public ProductViewModel Product { get; set; }
    }
}