using Company.Commerce.Data.EntityFramework.Context.Initializers;
using Company.Commerce.Data.EntityFramework.TypeMaps;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Company.Commerce.Data.EntityFramework.Context
{
    public class MainContext : DbContext, IDbContext
    {
        public MainContext() : this("MainContext") { }

        public MainContext(String nameOrConnectionString)
            : base(nameOrConnectionString)
        {
            Configuration.LazyLoadingEnabled = false;
            Configuration.ProxyCreationEnabled = false;
        }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.Configurations.Add(new AddressMap());
            modelBuilder.Configurations.Add(new CartItemMap());
            modelBuilder.Configurations.Add(new CategoryMap());
            modelBuilder.Configurations.Add(new CategoryImageXrefMap());
            modelBuilder.Configurations.Add(new OrderMap());
            modelBuilder.Configurations.Add(new OrderLineMap());
            modelBuilder.Configurations.Add(new ProductMap());
            modelBuilder.Configurations.Add(new ProductImageXrefMap());
            modelBuilder.Configurations.Add(new ProductCategoryXrefMap());
            modelBuilder.Configurations.Add(new UserAddressXrefMap());
            modelBuilder.Configurations.Add(new UserMap());

            base.OnModelCreating(modelBuilder);
        }

        public new IDbSet<TEntity> Set<TEntity>() where TEntity : class
        {
            return base.Set<TEntity>();
        }

        public new void SaveChanges()
        {
            base.SaveChanges();
        }

        public new IDbEntityEntry Entry<TEntity>(TEntity entity) where TEntity : class
        {
            return new DbEntityEntryWrapper(base.Entry<TEntity>(entity));
        }

        protected class DbEntityEntryWrapper : IDbEntityEntry
        {
            private DbEntityEntry _entry;

            public DbEntityEntryWrapper(DbEntityEntry entry)
            {
                _entry = entry;
            }

            public EntityState State
            {
                get
                {
                    return _entry.State;
                }
                set
                {
                    _entry.State = value;
                }
            }
        }
    }
}
