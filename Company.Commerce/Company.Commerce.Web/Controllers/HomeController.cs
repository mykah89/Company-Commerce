using Company.Commerce.Entity.Models;
using Company.Commerce.Web.ViewModels.General;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Company.Commerce.Web.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult About()
        {
            return View();
        }

        public ActionResult Contact()
        {
            ContactUsViewModel model = new ContactUsViewModel();

            model.BusinessAddress = new Address()
            {
                AddressLine1 = "123 Business Street",
                AddressLine2 = "Suite 104",
                City = "City",
                PostalCode = "12345",
                State = "NY"
            };

            return View(model);
        }
    }
}