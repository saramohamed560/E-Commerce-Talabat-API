using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Talabat.Core.Entities.OrderAggregate;

namespace Talabat.Core.Specifications.OrderSpecs
{
    public  class OrderSpecification :BaseSpecifications<Order>
    {
        public OrderSpecification(string email):base(O=>O.BuyerEmail==email)
        {
            Includes.Add(O => O.DeliveryMethod);
            Includes.Add(O => O.OrderItems);
            AddOrderByDesc(O => O.OrderDate);
        }
        public OrderSpecification(string email , int orderId):base(o=>o.BuyerEmail==email && o.Id==orderId)
        {
            Includes.Add(O => O.DeliveryMethod);
            Includes.Add(O => O.OrderItems);
        }
    }
}
