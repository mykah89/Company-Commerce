using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Company.Commerce.Web.ViewModels.Account
{
    public class EditEmailViewModel
    {
        [Compare("NewEmailAddress")]
        [Display(Name = "Confirm New Email Address")]
        [Required]
        public String ConfirmNewEmailAddress { get; set; }

        [Display(Name = "Current Email Address")]
        public String CurrentEmailAddress { get; set; }

        [Display(Name = "New Email Address")]
        [EmailAddress]
        [Required]
        public String NewEmailAddress { get; set; }

        [DataType(DataType.Password)]
        [Required]
        public String Password { get; set; }
    }
}