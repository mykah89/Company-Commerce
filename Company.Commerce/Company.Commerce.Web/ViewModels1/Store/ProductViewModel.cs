using Project.MVC.Entity.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Project.MVC.Web.ViewModels.Store
{
    public class ProductViewModel
    {
        public ProductViewModel()
        {
            ProductReviews = new List<ProductReviewViewModel>();
        }

        public Int32 ProductID { get; set; }
        public Int32 CategoryID { get; set; }

        public DateTime DateAdded { get; set; }

        public DefaultProductImage DefaultImage { get; set; }

        public String ProductName { get; set; }

        public ICollection<ProductReviewViewModel> ProductReviews { get; set; }

        public Decimal UnitPrice { get; set; }
        public Int32 UnitsInStock { get; set; }

        public Boolean Discontinued { get; set; }

        public virtual CategoryViewModel Category { get; set; }

        public virtual List<ProductImage> Images { get; set; }
    }
}