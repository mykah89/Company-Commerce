using Company.Commerce.Data.EntityFramework.Context;
using Company.Commerce.Data.EntityFramework.Context.Initializers;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Company.Commerce.Service.IntegrationTest
{
    [SetUpFixture]
    public class SetUpTearDown
    {
        private static MainContext _context;

        [SetUp]
        public void SetUp()
        {
            Database.SetInitializer<MainContext>(new DeleteCreateMainContextDbAlways());

            _context = new MainContext(TestVariables.ConnectionString);

            _context.Database.Initialize(true);

            Database.SetInitializer<MainContext>(null);
        }
    }
}
