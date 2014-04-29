using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Company.Commerce.Data
{
    public interface IDbContext : IDisposable
    {
        IDbSet<TEntity> Set<TEntity>()
            where TEntity : class;

        IDbEntityEntry Entry<TEntity>(TEntity entity)
            where TEntity : class;

        void SaveChanges();
    }
}
