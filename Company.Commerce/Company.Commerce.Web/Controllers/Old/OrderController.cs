using Project.MVC.Service;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Project.MVC.Web.Controllers
{
    public class OrderController : Controller
    {
        private IOrderService _orderService;

        public OrderController(IOrderService orderService)
        {
            _orderService = orderService;
        }
        //
        // GET: /Order/
        //public ActionResult Index()
        //{
        //    return View();
        //}

        //
        // GET: /Order/OrderStatus
        public ActionResult OrderStatus(Int32 orderId)//String postalCode)
        {
            var order = _orderService.Get(orderId);

            return View(order);
        }

        //
        // POST: /Order/OrderStatus
        //[HttpPost]
        //[ValidateAntiForgeryToken]
        //public ActionResult OrderStatus(Int32 orderId)
        //{

        //    return View();
        //}

	}
}