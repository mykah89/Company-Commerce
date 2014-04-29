using PagedList;
using Project.MVC.Entity.Models;
using Project.MVC.Web.Models.Admin;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.WebPages.Html;

namespace Project.MVC.Web.ViewModels.Admin
{
    public class AdminOrdersViewModel
    {
        public AdminOrdersViewModel()
        {
            OrderUpdates = new List<String>();

            //OrderActions = new List<SelectListItem>();
        }

        public OrderAction OrderAction { get; set; }

        //public List<SelectListItem> OrderActions { get; set; }

        public IPagedList<Order> Orders { get; set; }

        public List<String> OrderUpdates { get; set; }
    }
}