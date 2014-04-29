using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Project.MVC.Web.ViewModels.ShoppingCart
{
    public class UpdateCartItemQuantityViewModel
    {
        public Int32 CartItemID { get; set; }

        [Required(ErrorMessage = "A quantity must be specified.")]
        public Int32 Quantity { get; set; }
    }
}