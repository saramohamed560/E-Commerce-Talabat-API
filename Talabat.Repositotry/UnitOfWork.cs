using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Metadata.Ecma335;
using System.Text;
using System.Threading.Tasks;
using Talabat.Core;
using Talabat.Core.Entities;
using Talabat.Core.Repositories.Contract;
using Talabat.Repositotry.Data;

namespace Talabat.Repositotry
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly StoreContext _dbContext;
        private Hashtable _repositories;

        public UnitOfWork(StoreContext dbContext)
        {
            _dbContext = dbContext;
            _repositories = new Hashtable();
        }
        public async Task<int> CompleteAsync()
        => await _dbContext.SaveChangesAsync();

        public async ValueTask DisposeAsync()
        => await _dbContext.DisposeAsync();


        public IGenericRepository<T> Repository<T>() where T : BaseEntity
        {
            var type=typeof(T).Name; //Product ,Order
            if (!_repositories.ContainsKey(type))
            {
            var Repository = new GenericRepository<T>(_dbContext);
            _repositories.Add(type, Repository);
            }
            return (IGenericRepository<T>) _repositories[type];
        }
    }
}
