using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using Talabat.Core.Entities;

namespace Talabat.Core.Specifications
{
    public interface ISpecification<T> where T: BaseEntity
    {
        //Signature for each property(each component (Expression for Linq Operator))
        //Where(P=>P.Id==id).Include(P => P.Brand).Include(P => P.Category).FirstOrDefaultAsync() as T;
        //1. Where(P=>P.Id==id)
        public Expression<Func<T,bool>>? Critria { get; set; }
        //2.List Of Includes
        public List<Expression<Func<T,object>>> Includes { get; set; }

        //order asc
        public Expression<Func<T,object>> OrderBy { get; set; }
        //order Desc
        public Expression<Func<T,object>> OrderByDesc { get; set; }

        public int Skip { get; set; }
        public int Take { get; set; }
        public bool IsPaginationEnabled { get; set; }


    }
}
