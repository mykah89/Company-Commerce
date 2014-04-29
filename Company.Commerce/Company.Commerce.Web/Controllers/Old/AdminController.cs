using Framework.Repository;
using Project.MVC.Service;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using PagedList;
using Project.MVC.Entity.Models;
using Project.MVC.Web.ViewModels.Admin;
using Project.MVC.Web.Models.Admin;

namespace Project.MVC.Web.Controllers
{
    [Authorize(Users = "mykah89@hotmail.com")]
    public class AdminController : Controller
    {
        private IUnitOfWork _uow;

        private ICreditCardService _creditCardService;
        private IOrderService _orderService;

        public AdminController(IUnitOfWork uow, ICreditCardService creditCardService, IOrderService orderService)
        {
            _uow = uow;
            _creditCardService = creditCardService;
            _orderService = orderService;
        }

        //
        // GET: /Admin/
        public ActionResult Index()
        {
            return View();
        }

        //
        // GET: /Admin/Orders
        public ActionResult Orders(Int32 pageNumber = 1)
        {
            var model = new AdminOrdersViewModel();

            object orderUpdates;

            if (TempData.TryGetValue("OrderUpdates", out orderUpdates))
            {
                model.OrderUpdates = orderUpdates as List<String>;
            }

            model.Orders = _orderService.GetAll().OrderByDescending(o => o.OrderDate).ToPagedList(pageNumber, 9);

            return View(model);
        }

        //
        // GET:/Admin/OrderAction/
        public ActionResult PerformOrderAction(OrderAction orderAction, Int32 orderID)
        {
            List<String> orderUpdates = new List<String>();

            if (orderAction == OrderAction.None)
            {
                orderUpdates.Add("Invalid action selection.");

                TempData.Add("OrderUpdates", orderUpdates);

                return RedirectToAction("Orders");
            }

            return View();
        }

        //
        // POST: /Admin/ChargeOrder
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult OrderCapturePayment(Int32 orderID)
        {
            var order = _orderService.GetWithTransactions(orderID);

            var authorizations = order.Transactions.Where(t => t.TransactionType == Entity.Models.TransactionType.Authorization);

            List<String> orderUpdates = new List<String>();

            if (authorizations.Any())
            {
                if (authorizations.Count() == 1)
                {
                    var transToCapture = authorizations.First() as CreditCardTransaction;

                    var result = _creditCardService.Capture(transToCapture);

                    if (result.Success)
                    {
                        transToCapture.TransactionType = TransactionType.Payment;

                        _orderService.Update(order);

                        _uow.Save();

                        orderUpdates.Add("Payment has been successfully captured.");
                    }
                    else
                        orderUpdates.AddRange(result.Errors);
                }
                else
                    orderUpdates.Add("More than one authorized transaction was found, please visit advanced options.");
            }
            else
                orderUpdates.Add("No transactions found to capture.");

            TempData.Add("OrderUpdates", orderUpdates);

            return RedirectToAction("Orders");
        }
    }
}