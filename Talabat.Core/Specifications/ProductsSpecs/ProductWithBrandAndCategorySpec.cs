using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using Talabat.Core.Entities;

namespace Talabat.Core.Specifications.ProductsSpecs
{
    public class ProductWithBrandAndCategorySpec :BaseSpecifications<Product>
    {
        //Using T Create object when  using GetAll Products
        public ProductWithBrandAndCategorySpec(ProductSpecParams specParams) 
            :base(P=>
                     (string.IsNullOrEmpty(specParams.Search)||P.Name.ToLower().Contains(specParams.Search))&&
                     (!specParams.BrandId.HasValue   || P.BrandId==specParams.BrandId)&&
                     (!specParams.CategoryId.HasValue ||P.CategoryId==specParams.CategoryId )
                 )
        {

            //Sorting
            if (!string.IsNullOrEmpty(specParams.Sort))
            {
                switch (specParams.Sort)
                {
                    case "priceAsc":
                        AddOrderBy(P => P.Price);
                        break;
                    case "priceDesc":
                        AddOrderByDesc(P => P.Price);
                        break;
                    default:
                        AddOrderBy(P => P.Name);
                        break;

                }
            }
            else
                AddOrderBy(P => P.Name);
            //Products     = 18 ~ 20
            //PageSize    5
            //Page Index 3 //skip(10).Take(5)

            //Pagination

            ApplyPagination((specParams.PageIndex - 1) * specParams.PageSize, specParams.PageSize);

            //Includes
           
            Includes.Add(P => P.Brand);
            Includes.Add(P => P.Category);


        }

        //Using to create object when using GetBYId Products
        public ProductWithBrandAndCategorySpec(int id) :base(P=>P.Id==id)
        {
            Includes.Add(P => P.Brand);
            Includes.Add(P => P.Category);
        }



    }
}
