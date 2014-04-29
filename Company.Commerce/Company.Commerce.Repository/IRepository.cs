using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Company.Commerce.Repository
{
    public interface IRepository<TEntity>
        where TEntity : class
    {
        TEntity Add(TEntity entity);

        void Delete(TEntity entity);

        void Insert(TEntity entity);

        IRepositoryQuery<TEntity> Query();

        void Update(TEntity entity);

        TEntity Find(params object[] keyValues);
    }
}
