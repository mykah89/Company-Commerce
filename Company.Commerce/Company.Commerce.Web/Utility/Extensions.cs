using Company.Commerce.Service.Validation;
using Company.Commerce.Web.Helpers;
using Company.Commerce.Web.ViewModels.Checkout;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Web;
using System.Web.Mvc;

namespace Company.Commerce.Web.Utility
{
    public static class Extensions
    {
        public static void MergeErrors(this ModelStateDictionary modelState, List<ValidationError> errors)
        {
            if (errors == null)
                throw new ArgumentNullException("errors");

            foreach (var error in errors)
            {
                if (!String.IsNullOrWhiteSpace(error.PropertyName))
                    modelState.AddModelError(error.PropertyName, error.Error);
                else
                    modelState.AddModelError("", error.Error);
            }
        }

        public static List<ValidationError> ToModelErrors(this List<ValidationError> errors)
        {
            if (errors == null)
                throw new ArgumentNullException("errors");

            List<ValidationError> result = new List<ValidationError>();

            errors.ForEach(ve => result.Add(new ValidationError("", ve.Error, ve.AttemptedValue)));

            return result;
        }

        public static void AddErrorForProperty<TModel>(this ModelStateDictionary modelState, Expression<Func<TModel, object>> expression, String errorMessage)
            where TModel : class
        {
            if (String.IsNullOrWhiteSpace(errorMessage))
                throw new ArgumentNullException("errorMessage");

            MemberExpression memberExpression = ExpressionHelpers.GetMemberExpression<TModel>(expression);

            modelState.AddModelError(memberExpression.Member.Name.ToString(), errorMessage);
        }

        public static IEnumerable<Month> Months(this DateTime dt)
        {
            return Enum.GetValues(typeof(Month)).Cast<Month>();
        }
    }
}