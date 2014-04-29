using Company.Commerce.Entity.Models;
using PagedList;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Company.Commerce.Web.ViewModels.Account
{
    [Bind(Exclude = "Orders")]
    public class OrderHistoryViewModel
    {
        public OrderHistoryViewModel()
        {
            Orders = new PagedList<Order>(new List<Order>(), 1, 10);
            PageNumber = 1;
        }

        [DataType(DataType.Date)]
        //[DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        public DateTime? From { get; set; }

        public IPagedList<Order> Orders { get; set; }

        public Int32 PageNumber { get; set; }

        [DataType(DataType.Date)]
        //[DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        public DateTime? To { get; set; }

    }
}