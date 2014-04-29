using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Company.Commerce.Service.IntegrationTest
{
    class NUnitTestRunner
    {
        [STAThread]
        static void Main(string[] args)
        {
            String[] testArgs = { Assembly.GetExecutingAssembly().Location };

            NUnit.ConsoleRunner.Runner.Main(testArgs);
        }
    }
}
