using Company.Commerce.Entity.Models;
using PagedList;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Company.Commerce.Web.ViewModels.Store
{
    public class StoreIndexViewModel
    {
        public Category DisplayCategory { get; set; }

        public IEnumerable<Category> DisplayCategories { get; set; }
    }
}