using Company.Commerce.Entity.Models;
using Company.Commerce.Repository;
using Company.Commerce.Service;
using Company.Commerce.Web.ViewModels.Store;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using PagedList;

namespace Company.Commerce.Web.Controllers
{
    public class StoreController : Controller
    {
        private ICategoryService _categoryService;

        private IProductService _productService;

        private readonly IUnitOfWork _uow;

        public StoreController(IUnitOfWork uow, ICategoryService categoryService, IProductService productService)
        {
            _categoryService = categoryService;

            _productService = productService;

            _uow = uow;
        }

        //
        // GET: /Store/
        public ActionResult Index()
        {
            //StoreIndexViewModel model = new StoreIndexViewModel();

            //IEnumerable<Category> allCategories = _categoryService.GetAll();

            ////Get 5 random categories
            //Random random = new Random();

            //Int32 seed = random.Next();

            //allCategories = allCategories.OrderBy(s => (~(s.CategoryId & seed)) & (s.CategoryId | seed)); // ^ seed);

            //model.DisplayCategory = allCategories.First();

            //model.DisplayCategories = allCategories;

            //setCurrentCategoryId(model.DisplayCategory.CategoryId);

            setCurrentCategoryId(0);

            return View();
        }

        //
        // GET: /Store/CategoryIndex?categoryID=
        public ActionResult Category(Int32 categoryId)
        {
            if (categoryId == 0)
                return RedirectToAction("Index");

            Category category = _categoryService.GetWithChildCategoriesAndImages(categoryId);

            if (category == null)
                return new HttpStatusCodeResult(System.Net.HttpStatusCode.NotFound);

            CategoryIndexViewModel model = new CategoryIndexViewModel()
            {
                CurrentCategory = category
            };

            setCurrentCategoryId(categoryId);

            return View("Category", model);
        }


        ////
        //// Get: /Store/CategoryDetails/?categoryId=X
        //public ActionResult CategoryDetails(Int32 categoryId = 0)
        //{
        //    if (categoryId == 0)
        //        return RedirectToAction("Index");

        //    var cat = _categoryService.GetCategoryWithChildCategories(categoryId);

        //    if (cat == null)
        //        return RedirectToAction("Index");

        //    ViewBag.CurrentCategoryID = cat.CategoryID;

        //    if (cat.ChildCategories.Any())
        //    {
        //        //var model = new BranchCategoryIndexViewModel();
        //        //model.CurrentCategory = Mapper.Map<CategoryViewModel>(cat);
        //        //model.Categories = Mapper.Map<List<CategoryViewModel>>(cat.ChildCategories.Take(3));
        //        return View("BranchCategory");//, model);
        //    }
        //    else
        //    {
        //        var model = new LeafCategoryViewModel();
        //        model.CurrentCategory = Mapper.Map<CategoryViewModel>(cat);
        //        //TODO take random products
        //        model.Products = Mapper.Map<List<ProductViewModel>>(_prodService.GetAllByCategoryIdWithImages(cat.CategoryID).Take(3));

        //        return View("LeafCategory", model);
        //    }
        //}

        ////
        //// GET: /Store/AllCategories/?categoryId=X
        //public ActionResult AllCategories(Int32 parentCategoryId = 0, Int32 pageNumber = 1)
        //{
        //    ViewBag.CurrentCategoryID = parentCategoryId;

        //    IPagedList<CategoryViewModel> cats;

        //    if (parentCategoryId == 0)
        //        cats = Mapper.Map<List<CategoryViewModel>>(_categoryService.GetAll()).ToPagedList(pageNumber, 9);
        //    else
        //        cats = Mapper.Map<List<CategoryViewModel>>(_categoryService.GetAll().Where(c => c.ParentCategoryID == parentCategoryId)).ToPagedList(pageNumber, 9);

        //    return View(cats);
        //}

        //
        // GET: /Store/AllProducts/?categoryId=
        public ActionResult AllProducts(AllProductsViewModel model)
        {
            IEnumerable<Product> products;

            if (model.CategoryId != 0)
                products = _productService.GetByCategoryIdWithImages(model.CategoryId);
            else
                products = _productService.GetAllWithImages();

            if (!products.Any())
                return RedirectToAction("Index");

            //Ordering
            if (model.OrderBy != ProductOrderingOptions.None)
            {
                if (!model.OrderByDescending)
                {
                    if (model.OrderBy == ProductOrderingOptions.DateAdded)
                        products = products.OrderBy(p => p.Price);

                    if (model.OrderBy == ProductOrderingOptions.Price)
                        products = products.OrderBy(p => p.Price);
                }
                else
                {
                    if (model.OrderBy == ProductOrderingOptions.DateAdded)
                        products = products.OrderByDescending(p => p.Price);

                    if (model.OrderBy == ProductOrderingOptions.Price)
                        products = products.OrderByDescending(p => p.Price);
                }
            }
            else
                products = products.OrderBy(p => p.UnitsInStock);

            model.ProductsList = ((IOrderedEnumerable<Product>)products).ToPagedList(model.PageNumber, 9);

            model.ProductOrderingOptions = Enum.GetValues(typeof(ProductOrderingOptions)).Cast<ProductOrderingOptions>().ToList();

            setCurrentCategoryId(model.CategoryId);

            return View(model);
        }

        ////
        //// GET: /Store/
        ////public ActionResult Index(Int32 categoryId = 0, Int32 pageNumber = 1)
        ////{
        ////    ViewBag.CurrentCategoryID = categoryId;

        ////    var allCategories = _categoryService.GetAll();

        ////    if (categoryId != 0 && allCategories.FirstOrDefault(c => c.CategoryID == categoryId) == null)
        ////        return new HttpStatusCodeResult(404);

        ////    var sivm = new StoreIndexViewModel();

        ////    //Default value, categoryId not set.
        ////    if (categoryId == 0)
        ////    {
        ////        sivm.CurrentCategory = Mapper.Map<CategoryViewModel>(new Category { CategoryID = 0, CategoryName = "All" });
        ////        sivm.DisplayCategories = Mapper.Map<List<CategoryViewModel>>(allCategories).ToPagedList(pageNumber, 9);
        ////    }
        ////    else
        ////    {
        ////        sivm.CurrentCategory = Mapper.Map<CategoryViewModel>(allCategories.First(c => c.CategoryID == categoryId));
        ////        sivm.DisplayCategories = Mapper.Map<List<CategoryViewModel>>(allCategories.Where(c => c.ParentCategoryID == categoryId)).ToPagedList(pageNumber, 9);

        ////        if (!sivm.CurrentCategory.ChildCategories.Any())
        ////        {
        ////            var lcdvm = new LeafCategoryDetailsViewModel();

        ////            lcdvm.CurrentCategory = sivm.CurrentCategory;

        ////            //TEST
        ////            var categoryProducts = _prodService.GetAll();
        ////            //var categoryProducts = _prodService.GetAll().Where(p => p.CategoryId == lcdvm.CurrentCategory.Id);

        ////            lcdvm.Products = Mapper.Map<List<ProductViewModel>>(categoryProducts).ToPagedList(pageNumber, 9);

        ////            ViewBag.TitleBarName = sivm.CurrentCategory.CategoryName;
        ////            ViewBag.TitleBarSubText = "Some subtext goes here.";

        ////            return View("LeafCategoryDetails", lcdvm);
        ////        }
        ////    }

        ////    ViewBag.TitleBarName = sivm.CurrentCategory.CategoryName;
        ////    ViewBag.TitleBarSubText = "Some subtext goes here.";

        ////    return View(sivm);
        ////}

        //
        // GET: /ProductDetails/?productId=x
        public ActionResult ProductDetails(Int32 productId, String returnUrl = "")
        {
            Product product = _productService.GetWithImagesAndCategories(productId);

            if (product == null)
                return HttpNotFound();

            if (!String.IsNullOrWhiteSpace(returnUrl))
                ViewBag.ReturnUrl = returnUrl;

            Int32 defaultCategoryId = product
                .AssociatedCategories.FirstOrDefault(ac => ac.IsDefault == true).CategoryId;

            setCurrentCategoryId(defaultCategoryId);

            return View(product);
        }

        ////UNDONE
        //[ChildActionOnly]
        //public ActionResult _FeaturedProducts(Int32 categoryID)
        //{
        //    //Look up some featured products for the specified category
        //    var model = _prodService.GetAllWithImages().Take(3);

        //    return PartialView(model);
        //}

        //[ChildActionOnly]
        //public ActionResult _NewItemsStore()
        //{
        //    var model = _prodService.GetAllWithImages().OrderByDescending(p => p.DateAdded).Take(4).ToList();

        //    return PartialView(model);
        //}

        //[ChildActionOnly]
        //public ActionResult _RelatedProducts()
        //{
        //    var model = Mapper.Map<List<ProductViewModel>>(_prodService.GetAllWithImages().Take(4));

        //    return PartialView(model);
        //}

        [ChildActionOnly]
        public ActionResult _CategoryPromotional(Int32 categoryId)
        {
            //List<CategoryPromotions> categoryPromotions =_categoryService.GetCurrentCategoryPromotions(categoryId);

            //if (categoryPromotions.Any())
            //return PartialView(categoryPromotions.First());

            return Content(String.Empty);
        }

        [ChildActionOnly]
        public ActionResult _StoreBreadCrumbs(Int32? currentCategoryId)
        {
            List<Category> model = new List<Category>();

            IEnumerable<Category> allCategories = _categoryService.GetAll();

            if (currentCategoryId.HasValue && currentCategoryId.Value != 0)
            {
                Category currentCat = allCategories
                    .Single(c => c.CategoryId == currentCategoryId.Value);

                if (currentCat == null)
                    throw new InvalidOperationException("Invalid Category ID specified for crumbs.");

                List<Category> roots = this.getRoots(currentCat, allCategories);

                roots.Reverse();

                model.AddRange(roots);

                model.Add(currentCat);
            }

            return PartialView("~/Views/Store/Shared/_StoreBreadCrumbs.cshtml", model);
        }

        [ChildActionOnly]
        public ActionResult _StoreNavigation(Int32? currentCategoryId)
        {
            IEnumerable<Category> allCategories = _categoryService.GetAll();

            Category model;

            if (currentCategoryId.HasValue && currentCategoryId.Value != 0)
            {
                model = allCategories.First(c => c.CategoryId == currentCategoryId.Value);
            }
            else
            {
                model = new Category() { Name = "Index" };

                model.ChildCategories = allCategories.Where(c => c.ParentCategoryId == null).ToList();
            }

            return PartialView("~/Views/Store/Shared/_StoreNavigation.cshtml", model);
        }


        ////UNDONE
        //[ChildActionOnly]
        //public ActionResult _StorePromotional()
        //{
        //    return PartialView();
        //}

        #region Helpers

        private List<Category> getRoots(Category category, IEnumerable<Category> allCategories)
        {
            if (category == null)
                throw new ArgumentNullException("category");

            if (allCategories == null)
                throw new ArgumentNullException("allCategories");

            var result = new List<Category>();

            if (category.ParentCategoryId == null)
                return result;

            while (category.ParentCategoryId != null)
            {
                var parentCategory = allCategories.Single(c => c.CategoryId == category.ParentCategoryId);

                if (parentCategory == null)
                    throw new InvalidOperationException("Parent category not null, but no parent category found.");

                result.Add(parentCategory);

                category = parentCategory;
            }

            return result;
        }

        private void setCurrentCategoryId(Int32 categoryId)
        {
            ViewBag.CurrentCategoryId = categoryId;
        }

        #endregion
    }
}