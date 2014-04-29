using Company.Commerce.Data.EntityFramework.Context;
using Company.Commerce.Data.EntityFramework.Context.Initializers;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;

namespace Company.Commerce.Web.App_Start
{
    public static class DataConfig
    {
        public static void Initialize()
        {
#if DEBUG
            Database.SetInitializer<MainContext>(new DeleteCreateMainContextDbAlways());
#else
            Database.SetInitializer<MainContext>(new MainContextInitializer());
#endif
        }
    }
}