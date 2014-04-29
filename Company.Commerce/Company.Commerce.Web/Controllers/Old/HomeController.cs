using Project.MVC.Entity.Models.Configuration;
using Project.MVC.Service;
using Project.MVC.Web.ViewModels.General;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Project.MVC.Web.Controllers
{
    public class HomeController : Controller
    {
        private IAppInfoService _appInfoService;

        public HomeController(IAppInfoService appInfoService)
        {
            _appInfoService = appInfoService;
        }

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
            var model = new ContactUsViewModel();

            model.BusinessAddress = _appInfoService.GetBusinessAddress();

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Contact(ContactUsViewModel model)
        {
            if (ModelState.IsValid)
            {
                //Send contact email
                //UNDONE
                //dynamic Email = new Email("~/Views/Emails/Home/Contact.cshtml");
                //Email.To = "mykah89@hotmail.com";
                //Email.ReturnEmail = model.Email;
                //Email.Content = model.Text;
                //Email.Send();

                return View("ContactSuccess");
            }
            else //Since we use this in multiple places we dont know where it came from
                //redirect to a generic page with errors so the user can try again.
                return View(model);
        }

        public ActionResult FAQ()
        {
            return View();
        }

        public ActionResult Support()
        {
            return View();
        }

        #region Helpers
        private BusinessAddress getBusinessAddress()
        {
            var result = new BusinessAddress();

            var s = ConfigurationManager.GetSection("businessAddress");

            //result.Name = ConfigurationManager.AppSettings.Get("BusinessAddressName");
            //result.AddressLine1 = ConfigurationManager.AppSettings.Get("BusinessAddressAddressLine1");
            //result.AddressLine2 = ConfigurationManager.AppSettings.Get("BusinessAddressAddressLine2");
            //result.City = ConfigurationManager.AppSettings.Get("BusinessAddressCity");
            //result.Email = ConfigurationManager.AppSettings.Get("BusinessAddressEmail");
            //result.PostalCode = ConfigurationManager.AppSettings.Get("BusinessAddressPostalCode");
            //result.State = ConfigurationManager.AppSettings.Get("BusinessAddressState");

            return result;
        }
        #endregion
    }
}
