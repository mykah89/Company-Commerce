using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Caching;
using System.Web;

namespace Company.Commerce.Web.Settings
{
    public static class CheckoutSettings
    {
        static CheckoutSettings()
        {
            CheckoutInstanceCachePolicy = new CacheItemPolicy();
            CheckoutInstanceCachePolicy.SlidingExpiration = TimeSpan.FromMinutes(10);

            CheckoutIsEnabled = true;

            SalesTaxRate = .0875m;
        }

        public static CacheItemPolicy CheckoutInstanceCachePolicy { get; private set; }

        public static Boolean CheckoutIsEnabled { get; private set; }

        public static Decimal SalesTaxRate { get; set; }
    }
}