using Company.Commerce.Entity.Models;
using Company.Commerce.Service.Utility;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Company.Commerce.Service.IntegrationTest
{
    public class FakeProductService : IProductService
    {
        public Product Get(Int32 productId)
        {
            if (productId == 1)
            {
                return new Product()
                {
                    IsActive = true,
                    Name = "SomeProduct",
                    Price = 4.00m,
                    ProductId = productId,
                    UnitsInStock = 10
                };
            }

            return null;
        }

        public IEnumerable<Product> GetAllWithImages()
        {
            throw new NotImplementedException();
        }

        public Product GetWithImagesAndCategories(int productId)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<Product> GetByCategoryIdWithImages(Int32 categoryId)
        {
            throw new NotImplementedException();
        }

        public Product GetWithImages(Int32 productId)
        {
            throw new NotImplementedException();
        }

        public Task<ServiceOperationResult<Product>> CreateAsync(Product product)
        {
            throw new NotImplementedException();
        }
    }
}
