using Company.Commerce.Entity.Models;
using Company.Commerce.Repository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Company.Commerce.Service
{
    public class CategoryService : ICategoryService
    {
        private readonly IUnitOfWork _uow;

        public CategoryService(IUnitOfWork uow)
        {
            _uow = uow;
        }

        public Category Get(Int32 categoryId)
        {
            return _uow.Repository<Category>().Find(categoryId);
        }

        public Category GetWithChildCategories(Int32 categoryId)
        {
            return _uow.Repository<Category>()
                .Query()
                .Filter(c => c.CategoryId == categoryId)
                .Include(c => c.ChildCategories)
                .Get()
                .FirstOrDefault();
        }

        public Category GetWithChildCategoriesAndImages(Int32 categoryId)
        {
            return _uow.Repository<Category>()
                .Query()
                .Filter(c => c.CategoryId == categoryId)
                .Include(c => c.ChildCategories)
                .Include(c => c.AssociatedImages.Select(ai => ai.Image))
                .Get()
                .FirstOrDefault();
        }

        public IEnumerable<Category> GetAll()
        {
            return _uow.Repository<Category>()
                .Query()
                .Get();
        }
    }
}
