using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Web;

namespace Company.Commerce.Web.Helpers
{
    public static class ExpressionHelpers
    {
        public static MemberExpression GetMemberExpression<TModel>(Expression<Func<TModel, object>> expression)
            where TModel : class
        {
            return ((MemberExpression)expression.Body) ?? ((MemberExpression)((UnaryExpression)expression.Body).Operand);
        }
    }
}