using FluentValidation.Results;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Company.Commerce.Service.Validation.FluentValidation
{
    public static class FluentExtensions
    {
        public static List<ValidationError> ToValidationErrorList(this IList<ValidationFailure> failures)
        {
            List<ValidationError> result = new List<ValidationError>();

            foreach (var vFailure in failures)
            {
                result.Add(new ValidationError(vFailure.PropertyName, vFailure.ErrorMessage, vFailure.AttemptedValue));
            }

            return result;
        }
    }
}
