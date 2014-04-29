using FluentValidation.Attributes;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using Project.MVC.Web.ViewModels.General;
using System.Web.Mvc;
using Project.MVC.Web.Validators.Checkout;
using FluentValidation.Mvc;
using Project.MVC.Web.Binders.Checkout;
using Project.MVC.Web.ViewModels.ShoppingCart;
using Project.MVC.Web.Validators.Shared;
using Project.MVC.Entity.Models;

namespace Project.MVC.Web.ViewModels.Checkout.Steps
{
    [Validator(typeof(ShippingInformationViewModelValidator))]
    [ModelBinder(typeof(ShippingInformationViewModelBinder))]
    [Bind(Exclude = "AvailableAddresses, CurrentUserID")]
    public class ShippingInformationViewModel : CheckoutBaseViewModel
    {
        public ShippingInformationViewModel()
        {
            AvailableAddresses = new List<Address>();
            //ShippingAddress = new AddressViewModel();
        }


        [Display(Name = "Address")]
        public virtual String AddressLine1 { get; set; }

        [Display(Name = "Address 2")]
        public virtual String AddressLine2 { get; set; }

        //Not bound
        public IEnumerable<Address> AvailableAddresses { get; set; }

        public virtual String City { get; set; }

        //Not bound
        public Int32 CurrentUserID { get; set; }

        public String Email { get; set; }

        [Display(Name = "Phone")]
        public String PhoneNumber { get; set; }

        [Display(Name = "Zip")]
        public virtual String PostalCode { get; set; }

        public Int32 SelectedShippingAddressId { get; set; }

        public virtual String State { get; set; }
    }
}