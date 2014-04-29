using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Company.Commerce.Repository
{
    public interface IUnitOfWork 
    {
        IRepository<TEntity> Repository<TEntity>()
            where TEntity : class;

        void Commit();
    }
}
