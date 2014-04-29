using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Company.Commerce.Framework.Extensions
{
    public static class ExpressionExtensions
    {
        public static PropertyInfo GetPropertyInfoFromMemberExpression<TModel>(this Expression<Func<TModel, object>> expression)
            where TModel : class
        {
            if (expression == null)
                throw new ArgumentNullException("expression");

            MemberExpression memEx = expression.Body as MemberExpression
                ?? ((UnaryExpression)expression.Body).Operand as MemberExpression;

            if (memEx == null)
                throw new InvalidOperationException("Expression body must be a member expression.");

            return memEx.Member as PropertyInfo;
        }
    }
}
