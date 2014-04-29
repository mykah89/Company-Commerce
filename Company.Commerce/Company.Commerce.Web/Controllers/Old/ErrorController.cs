using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Project.MVC.Web.Controllers
{
    public class ErrorController : Controller
    {
        //
        // GET: /Error/
        public ActionResult Index()
        {
            ViewBag.ErrorMessage = "There was an error processing your request.";

            return View();
        }

        public ActionResult Error404()
        {
            ViewBag.ErrorMessage = "The resource you are looking for could not be found.";

            return View();
        }

        public ActionResult Error500()
        {
            ViewBag.ErrorMessage = "There was a problem on our end processing your request.";

            return View();
        }
	}
}