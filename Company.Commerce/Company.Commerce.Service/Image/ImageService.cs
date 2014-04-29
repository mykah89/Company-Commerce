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
    public class ImageService : IImageService
    {
        private readonly FluentImageValidator _imageValidator;

        private readonly IUnitOfWork _uow;

        public ImageService(IUnitOfWork uow)
        {
            _uow = uow;
        }

        public ServiceOperationResult<Image> Create(Image image)
        {
            if (image == null)
                throw new ArgumentNullException("image");

            ServiceOperationResult<Image> result = new ServiceOperationResult<Image>();

            ValidationResult validationResult = _imageValidator.Validate(image);

            if (validationResult.IsValid)
            {
                result.Data = _uow.Repository<Image>().Add(image);

                result.Succeeded = true;
            }
            else
                result.Errors.AddRange(validationResult.Errors.ToValidationErrorList());

            return result;
        }
    }
}
