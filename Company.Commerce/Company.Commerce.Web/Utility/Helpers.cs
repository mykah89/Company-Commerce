using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Security.Principal;
using System.Web;
using System.Web.Mvc;

namespace Company.Commerce.Web.Utility
{
    public static class Helpers
    {
        public static class ShoppingCartHelpers
        {
            private const String KeySessionShoppingCartId = "ShoppingCartId";

            public static String GetShoppingCartId(ControllerContext controllerContext)
            {
                if (controllerContext == null)
                    throw new ArgumentNullException("controllerContext");

                String existingCartId = controllerContext.HttpContext.Session[KeySessionShoppingCartId] as String;

                if (existingCartId != null)
                    return existingCartId;

                String shoppingCartId;

                if (controllerContext.HttpContext.User.Identity.IsAuthenticated)
                    shoppingCartId = ((ClaimsIdentity)controllerContext.HttpContext.User.Identity)
                        .Claims.FirstOrDefault(c => c.Type == ClaimTypes.Sid).Value.ToString();
                else
                    shoppingCartId = Guid.NewGuid().ToString("N");

                controllerContext.HttpContext.Session.Add(KeySessionShoppingCartId, shoppingCartId);

                return shoppingCartId;
            }

            public static void SetShoppingCartId(ControllerContext controllerContext, String shoppingCartId)
            {
                if (controllerContext == null)
                    throw new ArgumentNullException("controllerContext");

                if (String.IsNullOrWhiteSpace(shoppingCartId))
                    throw new ArgumentNullException("shoppingCartId");

                String existingShoppingCartId = controllerContext.HttpContext.Session[KeySessionShoppingCartId] as String;

                if (existingShoppingCartId != null)
                    existingShoppingCartId = shoppingCartId.Trim();

                controllerContext.HttpContext.Session.Add(KeySessionShoppingCartId, shoppingCartId);
            }

            public static void ClearShoppingCartId(ControllerContext controllerContext)
            {
                if (controllerContext == null)
                    throw new ArgumentNullException("controllerContext");

                String existingShoppingCartId = controllerContext.HttpContext.Session[KeySessionShoppingCartId] as String;

                if (existingShoppingCartId != null)
                    controllerContext.HttpContext.Session.Remove(KeySessionShoppingCartId);
            }
        }

        public static class IdentityHelpers
        {
            public static Int32 GetUserId(IIdentity identity)
            {
                if (identity == null)
                    throw new ArgumentNullException("identity");

                ClaimsIdentity claimsIdentity = (ClaimsIdentity)identity;

                return Convert.ToInt32(claimsIdentity.FindFirst(ClaimTypes.Sid).Value);
            }
        }
    }
}