using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Company.Commerce.Repository
{
    public interface IRepositoryQuery<TEntity> 
        where TEntity : class
    {
        EfRepositoryQuery<TEntity> Filter(Expression<Func<TEntity, bool>> filter);
        EfRepositoryQuery<TEntity> OrderBy(Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>> orderBy);
        EfRepositoryQuery<TEntity> Include(Expression<Func<TEntity, object>> expression);
        IEnumerable<TEntity> GetPage(int page, int pageSize, out int totalCount);
        IQueryable<TEntity> Get();
        //Task<IEnumerable<TEntity>> GetAsync();
    }
}
