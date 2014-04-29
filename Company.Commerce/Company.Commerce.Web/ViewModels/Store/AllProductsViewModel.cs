using Company.Commerce.Entity.Models;
using PagedList;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Company.Commerce.Web.ViewModels.Store
{
    public enum ProductOrderingOptions
    {
        None,
        Price,
        DateAdded
    }

    [Bind(Exclude = "ProductsList")]
    public class AllProductsViewModel
    {
        public AllProductsViewModel()
        {
            CategoryId = 0;
            OrderBy = 0;
            OrderByDescending = false;
            PageNumber = 1;
        }

        public Int32 CategoryId { get; set; }

        public ProductOrderingOptions OrderBy { get; set; }

        public Boolean OrderByDescending { get; set; }

        public IEnumerable<ProductOrderingOptions> ProductOrderingOptions { get; set; }

        public Int32 PageNumber { get; set; }

        public IPagedList<Product> ProductsList { get; set; }
    }
}