using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Company.Commerce.Service.IntegrationTest
{
    public static class TestVariables
    {
        public static String ConnectionString;

        static TestVariables()
        {
            String currentPath = new Uri(Assembly.GetExecutingAssembly().CodeBase).LocalPath;

            String currentDirectory = Path.GetDirectoryName(currentPath);

            String databasePath = Path.Combine(currentDirectory + "\\TestDb.mdf");

            ConnectionString = @"Server=(localdb)\v11.0;Database=Test;Integrated Security=True;AttachDbFileName=" + databasePath;
        }
    }
}
