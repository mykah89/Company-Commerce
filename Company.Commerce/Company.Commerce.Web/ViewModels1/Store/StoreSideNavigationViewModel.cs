using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Project.MVC.Web.ViewModels.Store
{
    public class StoreSideNavigationViewModel
    {
        public StoreSideNavigationViewModel()
        {
            AllCategories = new List<CategoryViewModel>();
        }

        public ICollection<CategoryViewModel> AllCategories { get; set; }

        public Int32 SelectedCategoryID { get; set; }
    }
}