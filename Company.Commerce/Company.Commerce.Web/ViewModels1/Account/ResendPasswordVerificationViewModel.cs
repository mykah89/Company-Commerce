﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Project.MVC.Web.ViewModels.Account
{
    public class ResendPasswordVerificationViewModel
    {
        [Required]
        [EmailAddress]
        public String Email { get; set; }
    }
}