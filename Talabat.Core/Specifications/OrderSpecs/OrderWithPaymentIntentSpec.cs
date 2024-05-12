using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Talabat.Core.Entities.OrderAggregate;

namespace Talabat.Core.Specifications.OrderSpecs
{
    public class OrderWithPaymentIntentSpec:BaseSpecifications<Order>
    {
        public OrderWithPaymentIntentSpec(string paymentIntentId):base(o=>o.PaymentIntentId==paymentIntentId)
        {
            
        }
    }
}
