using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Talabat.Core.Entities;
using Talabat.Core.Repositories.Contract;
using Talabat.Core.Specifications;
using Talabat.Repositotry.Data;

namespace Talabat.Repositotry
{
    public class GenericRepository<T> : IGenericRepository<T> where T : BaseEntity
    {
        private readonly StoreContext dbContext;

        public GenericRepository(StoreContext dbContext)
        {
            this.dbContext = dbContext;
        }
        public async Task<IReadOnlyList<T>> GetAllAsync()
        {
            return await dbContext.Set<T>().ToListAsync();
        }


        public  async Task<T?> GetAsync(int id)
        {
            return await dbContext.Set<T>().FindAsync(id);
            //Find return object or null
        }
        public async Task<IReadOnlyList<T>> GetAllWithSpecAsync(ISpecification<T> spec)
        {
            return  await ApplySpecifications(spec).AsNoTracking().ToListAsync();
        }

        public  async Task<T?> GetEntityWithSpecAsync(ISpecification<T> spec)
        {
            return  await ApplySpecifications(spec).FirstOrDefaultAsync();

        }

        public async Task<int> GetCountAsync(ISpecification<T> spec)
        {
            return await ApplySpecifications(spec).CountAsync();
        }
        private  IQueryable<T>  ApplySpecifications(ISpecification<T>spec)
        {
            return  SpecificationEvaluator<T>.GetQuery(dbContext.Set<T>(), spec);
        }

        public async Task AddAsync(T item)
        {
            await dbContext.Set<T>().AddAsync(item);
        }

        public void Delete(T item)
        => dbContext.Set<T>().Remove(item);

        public void Update(T item)
        => dbContext.Set<T>().Update(item);
    }

    
}
