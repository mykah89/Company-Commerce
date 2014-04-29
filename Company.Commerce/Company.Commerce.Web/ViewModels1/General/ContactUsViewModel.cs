using Project.MVC.Entity.Models.Configuration;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Project.MVC.Web.ViewModels.General
{
    [Bind(Exclude = ("BusinessAddress"))]
    public class ContactUsViewModel
    {
        public BusinessAddress BusinessAddress { get; set; }

        [Required(ErrorMessage = "Required.")]
        [EmailAddress(ErrorMessage = "Invalid email address.")]
        public String Email { get; set; }

        [Required(ErrorMessage = "Required.")]
        public String Name { get; set; }

        [Required(ErrorMessage = "Required.")]
        public String Text { get; set; }
    }
}