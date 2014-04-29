using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Project.MVC.Web.ViewModels.Account
{
    public class ConfirmAccountViewModel
    {
        public String Email { get; set; }
        public String ConfirmationToken { get; set; }
    }
}