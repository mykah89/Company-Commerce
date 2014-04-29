using PagedList;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Project.MVC.Web.ViewModels.Store
{
    public class LeafCategoryViewModel
    {
        public CategoryViewModel CurrentCategory { get; set; }

        public List<ProductViewModel> Products { get; set; }
    }
}