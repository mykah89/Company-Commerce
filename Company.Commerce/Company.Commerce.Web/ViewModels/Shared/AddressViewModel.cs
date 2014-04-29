using Company.Commerce.Entity.Models;
using Company.Commerce.Web.ViewModels.Shared.Validators;
using FluentValidation.Attributes;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Company.Commerce.Web.ViewModels.Shared
{
    public class AddressViewModel
    {
        [Display(Name = "Address")]
        public String AddressLine1 { get; set; }

        [Display(Name = "Address 2")]
        public String AddressLine2 { get; set; }

        public String City { get; set; }

        [Display(Name = "Postal Code")]
        public String PostalCode { get; set; }

        public StateCode? State { get; set; }
    }
}