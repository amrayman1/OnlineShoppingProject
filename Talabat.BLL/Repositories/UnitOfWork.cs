using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Talabat.BLL.Interfaces;
using Talabat.DAL.Data;
using Talabat.DAL.Entities;
using Talabat.DAL.Entities.Data;

namespace Talabat.BLL.Repositories
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly StoreContext _context;
        private Hashtable _repsitories;

        public UnitOfWork(StoreContext context)
        {
            _context = context;
        }
        public async Task<int> Complete()
            => await _context.SaveChangesAsync();

        public async void Dispose()
            => await _context.DisposeAsync();

        public IGenericRepository<TEntity> Repository<TEntity>() where TEntity : BaseEntity
        {
            if (_repsitories is null)
                _repsitories = new Hashtable();

            var type = typeof(TEntity).Name;

            if (!_repsitories.ContainsKey(type))
            {
                var repository = new GenericRepository<TEntity>(_context);
                _repsitories.Add(type, repository);
            }

            return (IGenericRepository<TEntity>)_repsitories[type];
        }
    }
}
