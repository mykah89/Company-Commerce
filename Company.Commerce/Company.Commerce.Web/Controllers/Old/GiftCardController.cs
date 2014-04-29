using AutoMapper;
using Project.MVC.Service;
using Project.MVC.Web.Filters;
using Project.MVC.Web.Helpers;
using Project.MVC.Web.ViewModels.Checkout;
using Project.MVC.Web.ViewModels.General;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Web;
using System.Web.Mvc;

namespace Project.MVC.Web.Controllers
{
    public class GiftCardController : Controller
    {
        private IGiftCardService _giftCardService;

        public GiftCardController(IGiftCardService giftCardService)
        {
            _giftCardService = giftCardService;
        }

        //
        // GET: /GiftCard/
        public ActionResult Index()
        {
            return View();
        }

        [AjaxOnly]
        //[ValidateAntiForgeryToken]
        public JsonResult CheckBalance(String giftCardNumber, String giftCardPin)
        {
            JsonDataResultViewModel result = new JsonDataResultViewModel();

            if (String.IsNullOrEmpty(giftCardNumber) || String.IsNullOrEmpty(giftCardPin))
            {
                result.Success = false;
                result.Errors.Add("Some information provided was invalid.");

                return Json(result, JsonRequestBehavior.AllowGet);
            }

            var gc = _giftCardService.Get(giftCardNumber, giftCardPin);

            if (gc == null)
            {
                result.Success = false;

                result.Errors.Add("Gift card could not be found.");

                return Json(result, JsonRequestBehavior.AllowGet);
            }

            if (gc.Balance <= 0)
            {
                result.Success = false;

                result.Errors.Add("Gift card has no remaining balance.");

                return Json(result, JsonRequestBehavior.AllowGet);
            }

            var model = new GiftCardPaymentViewModel()
            {
                Balance = gc.Balance,
                GiftCardNumber = gc.GiftCardNumber,
                GiftCardPin = gc.GiftCardPin
            };

            //If we got here, no errors.
            result.Data = ViewHelpers.RenderViewToString(this.ControllerContext, "~/Views/Checkout/DisplayTemplates/DisplayGiftcardCheckout.cshtml", model);//PartialView("~/Views/Checkout/DisplayTemplates/DisplayGiftcardCheckout.cshtml", model);
            result.Success = true;

            return Json(result, JsonRequestBehavior.AllowGet);
        }
    }
}