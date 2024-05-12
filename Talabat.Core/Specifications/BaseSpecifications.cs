using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using Talabat.Core.Entities;

namespace Talabat.Core.Specifications
{
    public class BaseSpecifications<T> : ISpecification<T> where T : BaseEntity
    {
        //Contain Common Specs 
        public Expression<Func<T, bool>>? Critria { get; set; }
        public List<Expression<Func<T, object>>> Includes { get; set; } = new List<Expression<Func<T, object>>>();
        public Expression<Func<T, object>> OrderBy { get ; set; }
        public Expression<Func<T, object>> OrderByDesc { get ; set; }
        public int Skip { get ; set ; }
        public int Take { get ; set ; }
        public bool IsPaginationEnabled { get ; set ; }

        
        public BaseSpecifications()
        {
            //Critria = null;
        }
       
        public BaseSpecifications(Expression<Func<T, bool>> critria)
        {
            Critria = critria;
        }
        // Just Setters
        public void AddOrderBy(Expression<Func<T,object>> orderBy)
        {
            OrderBy = orderBy;
        }
        public void AddOrderByDesc(Expression<Func<T, object>> orderBy)
        {
            OrderByDesc = orderBy;
        }

        public void ApplyPagination (int skip ,int take)
        {
            IsPaginationEnabled = true;
            Skip = skip; 
            Take=take;
        }

    }
}