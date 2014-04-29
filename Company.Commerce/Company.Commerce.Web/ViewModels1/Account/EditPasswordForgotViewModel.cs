using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Project.MVC.Web.ViewModels.Account
{
    public class EditPasswordForgotViewModel
    {
        public EditPasswordForgotViewModel()
        {
            PasswordVerificationTokenExpired = false;
        }

        [Compare("NewPassword")]
        [Display(Name = "Confirm New Password")]
        [Required]
        public String ConfirmNewPassword { get; set; }

        public String ConfirmationToken { get; set; }

        [EmailAddress]
        [Required]
        public String Email { get; set; }

        [Required]
        [StringLength(100, ErrorMessage = "The {0} must be at least {2} characters long.", MinimumLength = 8)]
        [DataType(DataType.Password)]
        [Display(Name = "New Password")]
        public String NewPassword { get; set; }

        public Boolean PasswordVerificationTokenExpired { get; set; }
    }
}