using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using FluentValidation.Attributes;
using System.ComponentModel.DataAnnotations;

namespace Company.Commerce.Web.ViewModels.Account
{
    //[Validator(typeof(EditPasswordViewModelValidator))]
    public class EditPasswordViewModel
    {
        [Compare("NewPassword")]
        [DataType(DataType.Password)]
        [Display(Name = "Confirm Password")]
        [Required]
        public String ConfirmPassword { get; set; }

        [Display(Name = "Current Password")]
        [Required]
        public String CurrentPassword { get; set; }

        [DataType(DataType.Password)]
        [Display(Name = "New Password")]
        [Required]
        [StringLength(100, ErrorMessage = "The {0} must be at least {2} characters long.", MinimumLength = 8)]
        public String NewPassword { get; set; }
    }
}