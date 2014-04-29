using Company.Commerce.Data;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Company.Commerce.Repository
{
    public class EfRepository<TEntity> : IRepository<TEntity>
        where TEntity : class
    {
        private IDbContext _context;

        public EfRepository(IDbContext context)
        {
            _context = context;
        }

        public TEntity Add(TEntity entity)
        {
            return _context.Set<TEntity>().Add(entity);
        }

        public void Insert(TEntity entity)
        {
            _context.Set<TEntity>().Attach(entity);
        }

        public void Delete(TEntity entity)
        {
            _context.Set<TEntity>().Attach(entity);

            _context.Set<TEntity>().Remove(entity);
        }

        public void Update(TEntity entity)
        {
            _context.Set<TEntity>().Attach(entity);

            IDbEntityEntry entry = _context.Entry<TEntity>(entity);

            entry.State = EntityState.Modified;
        }

        public TEntity Find(params object[] keyValues)
        {
            return _context.Set<TEntity>().Find(keyValues);
        }

        public IRepositoryQuery<TEntity> Query()
        {
            return new EfRepositoryQuery<TEntity>(this);
        }

        internal IQueryable<TEntity> Get(Expression<Func<TEntity, bool>> filter = null,
            Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>> orderByQueryable = null,
            List<Expression<Func<TEntity, object>>> includeProperties = null, int? page = null, int? pageSize = null)
        {
            IQueryable<TEntity> query = _context.Set<TEntity>().AsQueryable();

            if (includeProperties != null)
                includeProperties.ForEach(i => query = query.Include(i));

            if (filter != null)
                query = query.Where(filter);

            if (orderByQueryable != null)
                query = orderByQueryable(query);

            if (page != null && pageSize != null)
                query = query
                    .Skip((page.Value - 1) * pageSize.Value)
                    .Take(pageSize.Value);

            return query;
        }
    }
}
