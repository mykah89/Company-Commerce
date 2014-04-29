using AutoMapper;
using Proj.MVC.Entities.Models.Store;
using Proj.MVC.Repository;
using Proj.MVC.Services;
using Proj.MVC.Services.Store;
using Proj.MVC.ViewModels.Store;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Proj.MVC.Helpers;
using PagedList;
using System.Text;

namespace Proj.MVC.Controllers
{
    public class StoreController : Controller
    {
        private ICategoryService _categoryService;
        private IProductService _prodService;
        private IProductReviewService _prodReviewService;

        public StoreController(ICategoryService catService, IProductService prodService, IProductReviewService prodReviewService)
        {
            _categoryService = catService;
            _prodService = prodService;
            _prodReviewService = prodReviewService;
        }

        //
        // GET: /Store/

        public ActionResult Index(Int32 categoryId = 0, Int32 pageNumber = 1)
        {
            var allCategories = _categoryService.GetAll();

            if (categoryId != 0 && allCategories.FirstOrDefault(c => c.Id == categoryId) == null)
                return new HttpStatusCodeResult(404);

            var sivm = new StoreIndexViewModel();

            //Default value, categoryId not set.
            if (categoryId == 0)
            {
                sivm.CurrentCategory = Mapper.Map<CategoryViewModel>(new Category { CategoryID = 0, Name = "All" });
                sivm.DisplayCategories = Mapper.Map<List<CategoryViewModel>>(allCategories).ToPagedList(pageNumber, 9);
            }
            else
            {
                sivm.CurrentCategory = Mapper.Map<CategoryViewModel>(allCategories.First(c => c.Id == categoryId));
                sivm.DisplayCategories = Mapper.Map<List<CategoryViewModel>>(allCategories.Where(c => c.ParentCategoryId == categoryId)).ToPagedList(pageNumber, 9);

                if (!sivm.CurrentCategory.ChildCategories.Any())
                {
                    var lcdvm = new LeafCategoryDetailsViewModel();

                    lcdvm.CurrentCategory = sivm.CurrentCategory;

                    //TEST
                    var categoryProducts = _prodService.GetAll();
                    //var categoryProducts = _prodService.GetAll().Where(p => p.CategoryId == lcdvm.CurrentCategory.Id);

                    lcdvm.Products = Mapper.Map<List<ProductViewModel>>(categoryProducts).ToPagedList(pageNumber, 9);

                    ViewBag.TitleBarName = sivm.CurrentCategory.Name;
                    ViewBag.TitleBarSubText = "Some subtext goes here.";

                    return View("LeafCategoryDetails", lcdvm);
                }
            }

            ViewBag.TitleBarName = sivm.CurrentCategory.Name;
            ViewBag.TitleBarSubText = "Some subtext goes here.";

            return View(sivm);
        }

        public ActionResult ProductDetails(Int32 productId)
        {
            var prod = _prodService.FindProductWithReviews(productId);

            if (prod == null)
                return View("ProductNotFound");

            var model = Mapper.Map<ProductViewModel>(prod);

            ViewBag.TitleBarName = prod.Name;

            ViewBag.TitleBarSubText = "Some subtext goes here.";

            return View(model);
        }

        public ActionResult Search(StoreSearchViewModel ssvm, Int32 pageNumber = 1)
        {
            IEnumerable<Product> results;

            if (ssvm.Location != 0)
            {
                var catFilter = new Func<Product, Boolean>(p => p.CategoryID == ssvm.Location);

                results = _prodService.GetAll().Where(catFilter);

            }
            else
                results = _prodService.GetAll();

            if (!String.IsNullOrEmpty(ssvm.Term))
            {
                results = results.Where(p => p.Name.Contains(ssvm.Term));
            }

            var srvm = new SearchResultsViewModel();

            srvm.ssvm = ssvm;

            srvm.Products = Mapper.Map<List<ProductViewModel>>(results).ToPagedList(pageNumber, 9);

            ssvm.SearchLocations = this.searchLocations();

            return View("SearchResults", srvm);
        }

        //public ActionResult CategoryDetails(String categoryName, StoreSearchViewModel SSVM, Int32 pageNumber = 1)
        //{
        //    //Make sure the category name refers to a valid entry in the database.
        //    var existingCat = _categoryService.FindByNameWChild(categoryName);

        //    if (existingCat == null)
        //        return Content("Category not found.");

        //    var existingCatVM = Mapper.Map<CategoryViewModel>(existingCat);

        //    //If there is no child categories, this is a leaf node
        //    if (!existingCat.ChildCategories.Any())
        //    {
        //        var lcdVM = new LeafCategoryDetailsViewModel();

        //        lcdVM.CurrentCategory = existingCatVM;

        //        List<Product> prods;

        //        if (!String.IsNullOrEmpty(SSVM.Term))
        //        {
        //            prods = _prodService.GetByCategoryId(1);

        //            prods = prods.Where(p => p.Name.Contains(SSVM.Term)).ToList();
        //        }
        //        else
        //        {
        //            prods = _prodService.GetByCategoryId(1);
        //        }

        //        lcdVM.Products = new PagedList<ProductViewModel>
        //                (Mapper.Map<List<ProductViewModel>>(prods),
        //                pageNumber,
        //                9);

        //        return View("LeafCategoryDetails", lcdVM);
        //    }

        //    var cdVM = new CategoryDetailsViewModel();

        //    cdVM.CurrentCategory = existingCatVM;

        //    cdVM.CategoriesList =
        //        new PagedList<CategoryViewModel>
        //            (Mapper.Map<List<CategoryViewModel>>(existingCat.ChildCategories),
        //            pageNumber,
        //            8);

        //    return View(cdVM);
        //}

        //
        // GET: /Store/ProductDetails

        //public ActionResult ProductDetails(Int32 id)
        //{
        //    var prod = _prodService.FindById(id);

        //    if (prod == null)
        //        return View("Index");

        //    return View(Mapper.Map(prod, new ProductDetailsViewModel()));
        //}

        //[ChildActionOnly]
        //public ActionResult CategoriesList()
        //{
        //    var cats = Mapper.Map<List<CategoryViewModel>>(_categoryService.GetAllTree());

        //    return PartialView(cats);
        //}

        //[ChildActionOnly]
        //public ActionResult _StoreNavigation(Int32 currentCatId = 0)
        //{
        //    var snVM = new StoreNavigationViewModel();

        //    snVM.CurrentCategoryId = currentCatId;

        //    snVM.Categories = Mapper.Map<List<CategoryViewModel>>(_categoryService.GetAll());

        //    return PartialView("_StoreNavigation", snVM);
        //}

        [ChildActionOnly]
        public ActionResult _BreadCrumbs(Int32 categoryId)
        {
            var allCategories = _categoryService.GetAll();

            var crumbCats = new List<Category>();

            List<CategoryViewModel> results;

            var cat = allCategories.FirstOrDefault(c => c.Id == categoryId);

            if (cat != null)
            {
                crumbCats.Add(cat);

                while (cat.ParentCategoryId != null)
                {
                    cat = allCategories.FirstOrDefault(c => c.Id == cat.ParentCategoryId);

                    if (cat == null)
                    {
                        break;
                    }

                    crumbCats.Add(cat);
                }
            }

            crumbCats.Reverse();

            results = Mapper.Map<List<CategoryViewModel>>(crumbCats);

            ViewBag.CurrentCategoryId = categoryId;

            return PartialView(results);
        }

        [ChildActionOnly]
        public ActionResult _FeaturedItems(Int32 categoryId = 0)
        {
            //Make sure the category is valid
            if (categoryId != 0 && _categoryService.GetAll().Where(c => c.Id == categoryId).FirstOrDefault() == null)
                throw new InvalidOperationException("Attempt to get featured items for invalid category.");

            List<ProductViewModel> results;

            //Get all featured products
            var allFeaturedProducts = _prodService.GetAll().Where(p => p.Featured == true && p.Stock > 0);

            //Filter by category
            var categoryFiltered = allFeaturedProducts
                .Where(p => (categoryId != 0) ? p.CategoryId == categoryId : true);

            //Make sure there is at least 4 to choose from.
            if (categoryFiltered.Count() < 4)
            {
                var temp = categoryFiltered.ToList();
                temp.AddRange(_prodService.GetAll().Take(4 - categoryFiltered.Count()));

                results = Mapper.Map<List<ProductViewModel>>(temp);
            }
            else
                results = Mapper.Map<List<ProductViewModel>>(categoryFiltered.Take(4));

            return PartialView(results);
        }

        [ChildActionOnly]
        public ActionResult _FrontPageItems()
        {
            var products = _prodService.GetAll().Take(8);

            var model = Mapper.Map<List<ProductViewModel>>(products);

            return PartialView(model);
        }

        [ChildActionOnly]
        public ActionResult _NewItems(Int32 quantity)
        {
            var newProducts = _prodService.GetAll().OrderByDescending(p => p.DateAdded).Take(quantity);

            var model = Mapper.Map<List<ProductViewModel>>(newProducts);

            return PartialView(model);
        }

        [ChildActionOnly]
        public ActionResult _SearchPartial()
        {
            //var vm = new LeafCategoryDetailsViewModel();
            var ssvm = new StoreSearchViewModel();

            ssvm.SearchLocations = this.searchLocations();


            return PartialView("_SearchPartial", ssvm);
        }

        [ChildActionOnly]
        public ActionResult _SideNavigation()
        {
            var allCategories = _categoryService.GetAll();

            var model = Mapper.Map<List<CategoryViewModel>>(allCategories);

            return PartialView(model);
        }

        protected override void Dispose(bool disposing)
        {
            //_uow.Dispose();

            base.Dispose(disposing);
        }

        private List<SelectListItem> searchLocations()
        {
            List<SelectListItem> items = new List<SelectListItem>();

            items.Add(new SelectListItem() { Text = "All", Value = "0", Selected = true });

            items.AddRange(_categoryService.GetAll().Where(c => c.ParentCategoryId == null).Select(c => new SelectListItem() { Text = c.Name, Value = c.Id.ToString() }));

            return items;
        }
    }
}
