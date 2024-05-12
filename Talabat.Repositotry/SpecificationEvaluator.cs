using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;

using System.Text;
using System.Threading.Tasks;
using Talabat.Core.Entities;
using Talabat.Core.Specifications;

namespace Talabat.Repositotry
{
    internal  static class SpecificationEvaluator<TEntity> where TEntity : BaseEntity
    {
        public static IQueryable<TEntity> GetQuery(IQueryable<TEntity> inputQuery,ISpecification<TEntity> spec)
        {
            var query = inputQuery; //dbContext.Set<TEntity>()
            if(spec.Critria is not null)
               query = query.Where(spec.Critria); 

            if(spec.OrderBy is not null)
                query = query.OrderBy(spec.OrderBy);
            else if (spec.OrderByDesc is not null)
                query=query.OrderByDescending(spec.OrderByDesc);

            if (spec.IsPaginationEnabled)
                query = query.Skip(spec.Skip).Take(spec.Take);
            //dbContext.Set<TEntity>().Where(P=>P.Id==1)
            //List Of Includes (Expressions)
            //1.P=>P.Brand
            //2.P=>P.Category

            query = spec.Includes.Aggregate(query, (currentQury, IncludeExpression) => currentQury.Include(IncludeExpression));
            //dbContext.Set<TEntity>().Where(P=>P.Id==1).Include(P=>P.Brand).Include(P=>P.Category)
            return query;
        }
    }
}
