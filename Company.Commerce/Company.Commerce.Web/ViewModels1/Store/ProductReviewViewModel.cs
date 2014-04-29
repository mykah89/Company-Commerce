using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Project.MVC.Web.ViewModels.Store
{
    public class ProductReviewViewModel
    {
        public DateTime ReviewDate { get; set; }

        public String ReviewText { get; set; }

        public Int32 ProductReviewID { get; set; }

        public virtual ProductViewModel Product { get; set; }

        public Int32 ProductID { get; set; }

        public Int32 Rating { get; set; }

        public String Username { get; set; }

        //public virtual UserViewModel User { get; set; }

        public Int32 UserID { get; set; }
    }
}