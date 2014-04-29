using PagedList;
using Project.MVC.Entity.Models;
using Project.MVC.Web.Models.Store;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Project.MVC.Web.ViewModels.Store
{
    [Bind(Exclude = "ProductsList")]
    public class AllProductsViewModel
    {
        public AllProductsViewModel()
        {
            CategoryID = 0;
            OrderBy = 0;
            OrderByDescending = false;
            PageNumber = 1;
        }

        public Int32 CategoryID { get; set; }

        public ProductOrderingOptions OrderBy { get; set; }

        public Boolean OrderByDescending { get; set; }

        public Int32 PageNumber { get; set; }

        public IPagedList<Product> ProductsList { get; set; }
    }
}