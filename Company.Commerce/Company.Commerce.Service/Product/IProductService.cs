using Company.Commerce.Entity.Models;
using Company.Commerce.Service.Utility;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Company.Commerce.Service
{
    public interface IProductService
    {
        Product Get(Int32 productId);

        Product GetWithImages(Int32 productId);

        Product GetWithImagesAndCategories(Int32 productId);

        IEnumerable<Product> GetAllWithImages();

        IEnumerable<Product> GetByCategoryIdWithImages(Int32 categoryId);

        Task<ServiceOperationResult<Product>> CreateAsync(Product product);
    }
}
