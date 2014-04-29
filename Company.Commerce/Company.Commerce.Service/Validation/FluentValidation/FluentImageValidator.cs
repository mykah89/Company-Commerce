using Company.Commerce.Entity.Models;
using FluentValidation;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Company.Commerce.Service.Validation.FluentValidation
{
    public class FluentImageValidator : AbstractValidator<Image>
    {
        public FluentImageValidator()
        {
            this.CascadeMode = CascadeMode.StopOnFirstFailure;

            RuleFor(t => t.ImagePath).NotEmpty().Length(0, 300)
                .Must(ReferToValidImagePath);
        }

        private Boolean ReferToValidImagePath(String imagePath)
        {
            Uri uri = new Uri(imagePath, UriKind.RelativeOrAbsolute);

            return File.Exists(uri.AbsolutePath);
        }
    }
}
