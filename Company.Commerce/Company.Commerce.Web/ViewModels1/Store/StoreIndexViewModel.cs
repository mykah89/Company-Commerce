using PagedList;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Project.MVC.Web.ViewModels.Store
{
    public class StoreIndexViewModel
    {
        public CategoryViewModel DisplayCategory { get; set; }

        public List<CategoryViewModel> DisplayCategories { get; set; }
    }
}