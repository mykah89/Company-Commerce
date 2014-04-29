using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Project.MVC.Web.ViewModels.Account
{
    public class AddNewAddressViewModel
    {
        [Required]
        [Display(Name = "Address")]
        public String AddressLine1 { get; set; }

        [Display(Name = "Address 2")]
        public String AddressLine2 { get; set; }

        [Required]
        public String City { get; set; }
        [Display(Name = "Zip")]

        [Required]
        public String PostalCode { get; set; }

        [Required]
        public String State { get; set; }
    }
}