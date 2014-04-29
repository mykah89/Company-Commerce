using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Project.MVC.Web.ViewModels.Account
{
    public class UserProfileViewModel
    {
        [Display(Name = "First Name")]
        public String FirstName { get; set; }

        [Display(Name = "Last Name")]
        public String LastName { get; set; }

        [Display(Name = "Phone")]
        public String PhoneNumber { get; set; }

        public Int32 UserID { get; set; }
    }
}