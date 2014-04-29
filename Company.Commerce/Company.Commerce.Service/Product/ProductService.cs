using Company.Commerce.Entity.Models;
using Company.Commerce.Repository;
using Company.Commerce.Service.Utility;
using Company.Commerce.Service.Validation.FluentValidation;
using FluentValidation.Results;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Company.Commerce.Service
{
    public class ProductService : IProductService
    {
        private readonly FluentProductValidator _productValidator;

        private readonly IUnitOfWork _uow;

        public ProductService(IUnitOfWork uow)
        {
            _productValidator = new FluentProductValidator();

            _uow = uow;
        }

        public async Task<ServiceOperationResult<Product>> CreateAsync(Product product)
        {
            if (product == null)
                throw new ArgumentNullException("product");

            if (product.ProductId > 0)
                throw new ArgumentOutOfRangeException("product.ProductId", product.ProductId, "Created product must not have a set ProductId");

            ServiceOperationResult<Product> result = new ServiceOperationResult<Product>();

            ValidationResult validationResult = _productValidator.Validate(product);

            if (validationResult.IsValid)
            {
                result.Data = _uow.Repository<Product>().Add(product);

                result.Succeeded = true;
            }
            else
                result.Errors.AddRange(validationResult.Errors.ToValidationErrorList());

            return result;
        }

        public Product Get(Int32 productId)
        {
            return _uow.Repository<Product>().Find(productId);
        }

        public IEnumerable<Product> GetAllWithImages()
        {
            return _uow.Repository<Product>()
                .Query()
                .Include(p => p.AssociatedImages)
                .Include(p => p.AssociatedImages.Select(ai => ai.Image))
                .Get();
        }

        public IEnumerable<Product> GetByCategoryIdWithImages(Int32 categoryId)
        {
            return _uow.Repository<Product>()
                .Query()
                .Filter(p => p.AssociatedCategories.FirstOrDefault(ac => ac.CategoryId == categoryId) != null)
                .Include(p => p.AssociatedCategories)
                .Include(p => p.AssociatedImages.Select(ai => ai.Image))
                .Get();
        }

        public Product GetWithImages(Int32 productId)
        {
            return _uow.Repository<Product>()
                .Query()
                .Filter(p => p.ProductId == productId)
                .Include(p => p.AssociatedImages)
                .Include(p => p.AssociatedImages.Select(ai => ai.Image))
                .Get()
                .FirstOrDefault();
        }

        public Product GetWithImagesAndCategories(Int32 productId)
        {
            return _uow.Repository<Product>()
                .Query()
                .Filter(p => p.ProductId == productId)
                .Include(p => p.AssociatedCategories)
                .Include(p => p.AssociatedImages)
                .Include(p => p.AssociatedImages.Select(ai => ai.Image))
                .Get()
                .FirstOrDefault();
        }
    }
}
