using Project.MVC.Entity.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Project.MVC.Web.ViewModels.Store
{
    public class CategoryViewModel
    {
        public CategoryViewModel()
        {
            ChildCategories = new List<CategoryViewModel>();
            Products = new List<ProductViewModel>();
        }

        public Int32 CategoryID { get; set; }
        public Int32? ParentCategoryID { get; set; }

        public String CategoryName { get; set; }

        public CategoryImage CategoryImage { get; set; }

        public CategoryViewModel ParentCategory { get; set; }
        public ICollection<CategoryViewModel> ChildCategories { get; set; }
        public ICollection<ProductViewModel> Products { get; set; }
    }
}