using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Project.MVC.Web.ViewModels.General
{
    public class AddressViewModel
    {
        //public Int32 AddressID { get; set; }

        [Required]
        [Display(Name = "Address")]
        public virtual String AddressLine1 { get; set; }

        [Display(Name = "Address 2")]
        public virtual String AddressLine2 { get; set; }

        [Required]
        public virtual String City { get; set; }

        [Display(Name = "Zip")]
        [Required]
        public virtual String PostalCode { get; set; }

        [Required]
        public virtual String State { get; set; }

        //public Int32? UserID { get; set; }
    }
}