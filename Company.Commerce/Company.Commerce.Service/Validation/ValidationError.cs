using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using System.Reflection;
using Company.Commerce.Framework.Extensions;

namespace Company.Commerce.Service.Validation
{
    public class ValidationError
    {
        public static ValidationError ValidationErrorForProperty<TEntity>(Expression<Func<TEntity, object>> expression,
            String errorMessage, object attemptedValue = null)
            where TEntity : class
        {
            PropertyInfo propInfo = expression.GetPropertyInfoFromMemberExpression();

            return new ValidationError(propInfo.Name, errorMessage, attemptedValue);
        }

        public ValidationError(String propertyName, String error, object attemptedValue = null)
        {
            if (propertyName == null)
                throw new ArgumentNullException("propertyName");

            if (String.IsNullOrWhiteSpace(error))
                throw new ArgumentNullException("error");

            AttemptedValue = attemptedValue;

            PropertyName = propertyName;

            Error = error;
        }

        public String PropertyName { get; private set; }

        public object AttemptedValue { get; private set; }

        public String Error { get; private set; }
    }
}
