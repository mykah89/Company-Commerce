using Company.Commerce.Entity.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Company.Commerce.Data.EntityFramework.Context.Initializers
{
    public class MainContextInitializer : IDatabaseInitializer<MainContext>
    {
        public virtual void InitializeDatabase(MainContext context)
        {
            if(!context.Database.Exists())
            {
                context.Database.Create();

                //Required because EF does not implement a [Unique] attribute.
                String emailAndUsernameUniqueConstraint = @"ALTER TABLE USERS ADD CONSTRAINT UniqueUsernameAndEmail UNIQUE(Username, EmailAddress)";

                //TODO Unique constraints for join tables and default values

                context.Database.ExecuteSqlCommand(emailAndUsernameUniqueConstraint);
            }
        }

        protected virtual void Seed(MainContext context) { }
    }
}
