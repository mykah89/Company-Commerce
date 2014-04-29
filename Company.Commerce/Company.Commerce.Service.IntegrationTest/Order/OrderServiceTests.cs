using Company.Commerce.Data;
using Company.Commerce.Data.EntityFramework.Context;
using Company.Commerce.Entity.Models;
using Company.Commerce.Repository;
using Company.Commerce.Service.Utility;
using Company.Commerce.Service.Validation.FluentValidation;
using FluentValidation.Results;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Transactions;
using Company.Commerce.Service.IntegrationTest.TestHelper;
using FluentValidation;

namespace Company.Commerce.Service.IntegrationTest
{
    [TestFixture]
    public class OrderServiceTests
    {
        private IDbContext context;

        private OrderService _orderService;

        private IProductService _productService;

        private IUnitOfWork _uow;

        [SetUp]
        public void Setup()
        {
            context = new MainContext(TestVariables.ConnectionString);

            _uow = new EfUnitOfWork(context);

            _productService = new ProductService(_uow);

            _orderService = new OrderService(_uow, _productService);
        }

        [TearDown]
        public void FixtureTearDown()
        {
            context.Dispose();
        }

        [Test]
        public void Fluent_OrderLine_Validator()
        {
            FluentOrderLineValidator validator = new FluentOrderLineValidator(_productService);

            ValidationResult validationResult;

            #region Valid
            OrderLine valid = validOrderLine();

            validationResult = validator.Validate(valid);

            Assert.IsTrue(validationResult.IsValid);
            #endregion

            #region Price
            //Scale of two {e.g X.00}
            OrderLine invalidPriceScale = validOrderLine();

            invalidPriceScale.Price = 1;

            validationResult = validator.Validate(invalidPriceScale);

            Assert.IsTrue(validationResult.Errors.ContainsErrorForProperty<OrderLine>(o => o.Price));
            #endregion

            #region ProductId
            //ProductId must refer to a valid product
            OrderLine invalidProductId = validOrderLine();

            invalidProductId.ProductId = -1;

            validationResult = validator.Validate(invalidProductId);

            Assert.IsTrue(validationResult.Errors.ContainsErrorForProperty<OrderLine>(o => o.ProductId));
            #endregion

            #region Quantity
            //Quantity > 0
            OrderLine invalidQuantity = validOrderLine();

            invalidQuantity.Quantity = 0;

            validationResult = validator.Validate(invalidQuantity);

            Assert.IsTrue(validationResult.Errors.ContainsErrorForProperty<OrderLine>(o => o.Quantity));
            #endregion
        }

        [Test]
        public void Fluent_Order_Validator()
        {
            FluentOrderValidator validator = new FluentOrderValidator(_productService);

            ValidationResult validationResult;

            #region RuleSet:Create

            #region Valid
            Order valid = validOrder();

            validationResult = validator.Validate(valid, ruleSet: "Create");

            Assert.IsTrue(validationResult.IsValid);
            #endregion

            #region OrderDate
            //Must not be in the future
            Order invalidOrderDate = validOrder();

            invalidOrderDate.OrderDate = DateTime.Now + TimeSpan.FromMinutes(10);

            validationResult = validator.Validate(invalidOrderDate, ruleSet: "Create");

            Assert.IsTrue(validationResult.Errors.ContainsErrorForProperty<Order>(o => o.OrderDate));

            #endregion

            #region OrderLine
            //Not Empty
            Order emptyOrderLines = validOrder();

            emptyOrderLines.OrderLines = new List<OrderLine>();

            validationResult = validator.Validate(emptyOrderLines, ruleSet: "Create");

            Assert.IsTrue(validationResult.Errors.ContainsErrorForProperty<Order>(o => o.OrderLines));

            //Must be valid
            Order invalidOrderLines = validOrder();

            invalidOrderLines.OrderLines.Add(invalidOrderLine());

            validationResult = validator.Validate(invalidOrderLines, ruleSet: "Create");

            Assert.IsTrue(validationResult.Errors.ContainsErrorForProperty<Order>(o => o.OrderLines));
            #endregion

            #region OrderStatus
            //Must be 'Created'
            Order invalidOrderStatus = validOrder();

            invalidOrderStatus.OrderStatus = OrderStatus.Paid;

            validationResult = validator.Validate(invalidOrderStatus, ruleSet: "Create");

            Assert.IsTrue(validationResult.Errors.ContainsErrorForProperty<Order>(o => o.OrderStatus));

            #endregion

            #endregion
        }

        [Test]
        public async void Create()
        {
            using (TransactionScope scope = new TransactionScope())
            {
                #region Valid Creates

                Order valid = validOrder();

                ServiceOperationResult<Order> result = await _orderService.CreateAsync(valid);

                _uow.Commit();

                Order created = await _orderService.GetAsync(result.Data.OrderId);

                Assert.IsTrue(result.Succeeded);
                Assert.AreEqual(0, result.Errors.Count());
                Assert.IsNotNull(created);

                #endregion

                #region Invalid Fails

                Order invalid = invalidOrder();

                result = await _orderService.CreateAsync(invalid);

                Assert.IsFalse(result.Succeeded);
                Assert.AreNotEqual(0, result.Errors);
                Assert.IsNull(result.Data);

                #endregion

                //Any transaction will roll back
            }
        }

        #region Helpers
        private Order validOrder()
        {
            return new Order()
            {
                OrderDate = DateTime.Now,
                OrderLines = new List<OrderLine>() { validOrderLine() },
                OrderStatus = OrderStatus.Created
            };
        }

        private Order invalidOrder()
        {
            return new Order();
        }

        private OrderLine validOrderLine()
        {
            return new OrderLine()
            {
                Price = 4.00m,
                ProductId = 1,
                Quantity = 1
            };
        }

        private OrderLine invalidOrderLine()
        {
            return new OrderLine();
        }
        #endregion
    }
}
