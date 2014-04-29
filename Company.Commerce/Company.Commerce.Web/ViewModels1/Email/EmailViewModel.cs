using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Project.MVC.Web.ViewModels.Email
{
    public class EmailViewModel
    {
        public String SendTo { get; set; }
        public String Body { get; set; }
        public String Subject { get; set; }
    }
}