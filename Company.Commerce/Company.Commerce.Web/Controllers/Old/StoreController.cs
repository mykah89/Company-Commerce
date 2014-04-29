using AutoMapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Text;
using Project.MVC.Service;
using Project.MVC.Web.ViewModels.Store;
using Project.MVC.Entity.Models;
using PagedList;
using Project.MVC.Web.Models.Store;

namespace Project.MVC.Web.Controllers
{
    public class StoreController : Controller
    {
        private ICategoryService _categoryService;
        private IProductService _prodService;
        //private IProductReviewService _prodReviewService;

        public StoreController(ICategoryService catService, IProductService prodService)//, IProductReviewService prodReviewService)
        {
            _categoryService = catService;
            _prodService = prodService;
            //_prodReviewService = prodReviewService;
        }

        //
        // GET: /Store/
        public ActionResult Index()
        {
            StoreIndexViewModel sivm = new StoreIndexViewModel();

            var allCategories = _categoryService.GetAll();

            //Get 5 random categories
            Random random = new Random();
            Int32 seed = random.Next();
            allCategories = allCategories.OrderBy(s => (~(s.CategoryID & seed)) & (s.CategoryID | seed)); // ^ seed);

            sivm.DisplayCategory = Mapper.Map<CategoryViewModel>(allCategories.Take(1).FirstOrDefault());
            sivm.DisplayCategories = Mapper.Map<List<CategoryViewModel>>(allCategories.Skip(1).Take(4));

            return View(sivm);
        }

        //
        // GET: /Store/CategoryIndex?categoryID=
        public ActionResult Category(Int32 categoryID)
        {
            if (categoryID == 0)
                return RedirectToAction("Index");

            var category = _categoryService.GetCategoryWithChildCategories(categoryID);

            if (category == null)
                return new HttpStatusCodeResult(System.Net.HttpStatusCode.NotFound);

            var categoryIndexModel = new CategoryIndexViewModel();

            categoryIndexModel.CurrentCategory = category;

            return View("CategoryIndex", categoryIndexModel);
        }

        //
        // Get: /Store/CategoryDetails/?categoryId=X
        public ActionResult CategoryDetails(Int32 categoryId = 0)
        {
            if (categoryId == 0)
                return RedirectToAction("Index");

            var cat = _categoryService.GetCategoryWithChildCategories(categoryId);

            if (cat == null)
                return RedirectToAction("Index");

            ViewBag.CurrentCategoryID = cat.CategoryID;

            if (cat.ChildCategories.Any())
            {
                //var model = new BranchCategoryIndexViewModel();
                //model.CurrentCategory = Mapper.Map<CategoryViewModel>(cat);
                //model.Categories = Mapper.Map<List<CategoryViewModel>>(cat.ChildCategories.Take(3));
                return View("BranchCategory");//, model);
            }
            else
            {
                var model = new LeafCategoryViewModel();
                model.CurrentCategory = Mapper.Map<CategoryViewModel>(cat);
                //TODO take random products
                model.Products = Mapper.Map<List<ProductViewModel>>(_prodService.GetAllByCategoryIdWithImages(cat.CategoryID).Take(3));

                return View("LeafCategory", model);
            }
        }

        //
        // GET: /Store/AllCategories/?categoryId=X
        public ActionResult AllCategories(Int32 parentCategoryId = 0, Int32 pageNumber = 1)
        {
            ViewBag.CurrentCategoryID = parentCategoryId;

            IPagedList<CategoryViewModel> cats;

            if (parentCategoryId == 0)
                cats = Mapper.Map<List<CategoryViewModel>>(_categoryService.GetAll()).ToPagedList(pageNumber, 9);
            else
                cats = Mapper.Map<List<CategoryViewModel>>(_categoryService.GetAll().Where(c => c.ParentCategoryID == parentCategoryId)).ToPagedList(pageNumber, 9);

            return View(cats);
        }

        //
        // GET: /Store/AllProducts/?categoryId=X
        public ActionResult AllProducts(AllProductsViewModel model)
        {
            ViewBag.CurrentCategoryID = model.CategoryID;

            IEnumerable<Product> products;

            if (model.CategoryID != 0)
                products = _prodService.GetAllByCategoryIdWithImages(model.CategoryID);
            else
                products = _prodService.GetAllWithImages();

            //Ordering
            if (model.OrderBy != ProductOrderingOptions.None)
            {
                if (!model.OrderByDescending)
                {
                    if (model.OrderBy == ProductOrderingOptions.DateAdded)
                        products = products.OrderBy(p => p.UnitPrice);

                    if (model.OrderBy == ProductOrderingOptions.Price)
                        products = products.OrderBy(p => p.UnitPrice);
                }
                else
                {
                    if (model.OrderBy == ProductOrderingOptions.DateAdded)
                        products = products.OrderByDescending(p => p.UnitPrice);

                    if (model.OrderBy == ProductOrderingOptions.Price)
                        products = products.OrderByDescending(p => p.UnitPrice);
                }
            }
            else
                products = products.OrderBy(p => p.UnitsInStock);

            model.ProductsList = ((IOrderedEnumerable<Product>)products).ToPagedList(model.PageNumber, 9);

            return View(model);
        }

        //
        // GET: /Store/
        //public ActionResult Index(Int32 categoryId = 0, Int32 pageNumber = 1)
        //{
        //    ViewBag.CurrentCategoryID = categoryId;

        //    var allCategories = _categoryService.GetAll();

        //    if (categoryId != 0 && allCategories.FirstOrDefault(c => c.CategoryID == categoryId) == null)
        //        return new HttpStatusCodeResult(404);

        //    var sivm = new StoreIndexViewModel();

        //    //Default value, categoryId not set.
        //    if (categoryId == 0)
        //    {
        //        sivm.CurrentCategory = Mapper.Map<CategoryViewModel>(new Category { CategoryID = 0, CategoryName = "All" });
        //        sivm.DisplayCategories = Mapper.Map<List<CategoryViewModel>>(allCategories).ToPagedList(pageNumber, 9);
        //    }
        //    else
        //    {
        //        sivm.CurrentCategory = Mapper.Map<CategoryViewModel>(allCategories.First(c => c.CategoryID == categoryId));
        //        sivm.DisplayCategories = Mapper.Map<List<CategoryViewModel>>(allCategories.Where(c => c.ParentCategoryID == categoryId)).ToPagedList(pageNumber, 9);

        //        if (!sivm.CurrentCategory.ChildCategories.Any())
        //        {
        //            var lcdvm = new LeafCategoryDetailsViewModel();

        //            lcdvm.CurrentCategory = sivm.CurrentCategory;

        //            //TEST
        //            var categoryProducts = _prodService.GetAll();
        //            //var categoryProducts = _prodService.GetAll().Where(p => p.CategoryId == lcdvm.CurrentCategory.Id);

        //            lcdvm.Products = Mapper.Map<List<ProductViewModel>>(categoryProducts).ToPagedList(pageNumber, 9);

        //            ViewBag.TitleBarName = sivm.CurrentCategory.CategoryName;
        //            ViewBag.TitleBarSubText = "Some subtext goes here.";

        //            return View("LeafCategoryDetails", lcdvm);
        //        }
        //    }

        //    ViewBag.TitleBarName = sivm.CurrentCategory.CategoryName;
        //    ViewBag.TitleBarSubText = "Some subtext goes here.";

        //    return View(sivm);
        //}

        //
        // GET: /ProductDetails/?productId=x
        public ActionResult ProductDetails(Int32 productId)
        {
            var model = _prodService.GetProductWithImagesAndReviews(productId);

            if (model == null)
                return HttpNotFound();

            ViewBag.CurrentCategoryID = model.Categories.FirstOrDefault();

            return View(model);
        }

        //UNDONE
        [ChildActionOnly]
        public ActionResult _FeaturedProducts(Int32 categoryID)
        {
            //Look up some featured products for the specified category
            var model = _prodService.GetAllWithImages().Take(3);

            return PartialView(model);
        }

        [ChildActionOnly]
        public ActionResult _NewItemsStore()
        {
            var model = _prodService.GetAllWithImages().OrderByDescending(p => p.DateAdded).Take(4).ToList();

            return PartialView(model);
        }

        [ChildActionOnly]
        public ActionResult _RelatedProducts()
        {
            var model = Mapper.Map<List<ProductViewModel>>(_prodService.GetAllWithImages().Take(4));

            return PartialView(model);
        }

        [ChildActionOnly]
        public ActionResult _StoreBreadCrumbs(Int32? currentCategoryID)
        {
            var model = new List<Category>();

            var categories = _categoryService.GetAll();

            if (currentCategoryID.HasValue && currentCategoryID.Value != 0)
            {
                var currentCat = categories
                    .Single(c => c.CategoryID == currentCategoryID.Value);

                if (currentCat == null)
                    throw new InvalidOperationException("Invalid Category ID specified for crumbs.");

                var roots = this.getRoots(currentCat, categories);

                roots.Reverse();

                model.AddRange(roots);

                model.Add(currentCat);
            }

            return PartialView("~/Views/Store/Shared/_StoreBreadCrumbs.cshtml", model);
        }

        [ChildActionOnly]
        public ActionResult _StoreNavigation(Int32? currentCategoryID)
        {
            Category model;

            if (currentCategoryID.HasValue && currentCategoryID.Value != 0)
            {
                model = _categoryService.GetCategoryWithChildCategories(currentCategoryID.Value);
            }
            else
            {
                model = new Category() { CategoryName = "Index" };

                model.ChildCategories = _categoryService.GetAll().Where(c => c.ParentCategoryID == null).ToList();
            }

            return PartialView("~/Views/Store/Shared/_StoreNavigation.cshtml", model);
        }

        //UNDONE
        [ChildActionOnly]
        public ActionResult _StorePromotional()
        {
            return PartialView();
        }

        #region Helpers
        private List<Category> getRoots(Category category, IEnumerable<Category> allCategories)
        {
            if (category == null)
                throw new ArgumentNullException("category");

            if (allCategories == null)
                throw new ArgumentNullException("allCategories");

            var result = new List<Category>();

            if (category.ParentCategoryID == null)
                return result;

            while (category.ParentCategoryID != null)
            {
                var parentCategory = allCategories.Single(c => c.CategoryID == category.ParentCategoryID);

                if (parentCategory == null)
                    throw new InvalidOperationException("Parent category not null, but no parent category found.");

                result.Add(parentCategory);

                category = parentCategory;
            }

            return result;
        }
        #endregion
    }
}
