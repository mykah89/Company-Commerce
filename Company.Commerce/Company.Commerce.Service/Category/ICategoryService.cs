using Company.Commerce.Entity.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Company.Commerce.Service
{
    public interface ICategoryService
    {
        Category Get(Int32 categoryId);

        Category GetWithChildCategories(Int32 categoryId);

        Category GetWithChildCategoriesAndImages(Int32 categoryId);

        IEnumerable<Category> GetAll();
    }
}
