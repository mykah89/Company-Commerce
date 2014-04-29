using Company.Commerce.Data;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Company.Commerce.Repository
{
    public class EfUnitOfWork : IUnitOfWork
    {
        #region Fields
        private IDbContext _context;

        private Hashtable _repositories;

        #endregion

        #region Properties

        #endregion

        #region Constructors
        public EfUnitOfWork(IDbContext context)
        {
            _context = context;
        }

        #endregion

        #region Public Methods

        public void Commit()
        {
            _context.SaveChanges();
        }

        public IRepository<TEntity> Repository<TEntity>() where TEntity : class
        {
            if (_repositories == null)
            {
                _repositories = new Hashtable();
            }

            Type entityType = typeof(TEntity);

            if (_repositories.ContainsKey(entityType))
            {
                return _repositories[entityType] as IRepository<TEntity>;
            }
            else
            {
                IRepository<TEntity> repository = new EfRepository<TEntity>(_context);

                _repositories.Add(entityType, repository);

                return repository;
            }
        }

        #endregion

        #region Private Methods

        #endregion
    }
}
