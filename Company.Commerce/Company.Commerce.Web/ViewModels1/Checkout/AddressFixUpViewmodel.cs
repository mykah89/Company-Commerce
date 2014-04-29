using Company.Commerce.Entity.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Project.MVC.Web.ViewModels.Checkout
{
    public class AddressFixUpViewmodel
    {
        public Address ExistingAddress { get; set; }
        public Address SuggestedAddress { get; set; }
    }
}