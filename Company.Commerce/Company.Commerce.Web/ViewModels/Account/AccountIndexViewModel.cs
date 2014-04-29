using Company.Commerce.Entity.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Company.Commerce.Web.ViewModels.Account
{
    public class AccountIndexViewModel
    {
        public AccountIndexViewModel()
        {
            ImportantAccountUpdates = new List<String>();
            RecentOrders = new List<Order>();
        }

        public List<String> ImportantAccountUpdates { get; set; }

        public IEnumerable<Order> RecentOrders { get; set; }

        public User User { get; set; }

        //public UserProfile UserProfile { get; set; }
    }
}