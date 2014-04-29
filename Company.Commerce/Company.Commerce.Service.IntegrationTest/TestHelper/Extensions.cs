using Company.Commerce.Service.Validation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Company.Commerce.Framework.Extensions;
using FluentValidation.Results;

namespace Company.Commerce.Service.IntegrationTest.TestHelper
{
    public static class Extensions
    {
        public static Boolean ContainsErrorForProperty<TEntity>(this List<ValidationError> errors, Expression<Func<TEntity, object>> expression)
            where TEntity : class
        {
            PropertyInfo propInfo = expression.GetPropertyInfoFromMemberExpression();

            return errors.FirstOrDefault(e => e.PropertyName == propInfo.Name) != null;
        }

        public static Boolean ContainsErrorForProperty<TEntity>(this IList<ValidationFailure> errors, Expression<Func<TEntity, object>> expression)
            where TEntity : class
        {
            PropertyInfo propInfo = expression.GetPropertyInfoFromMemberExpression();

            return errors.FirstOrDefault(e => e.PropertyName == propInfo.Name) != null;
        }
    }
}
