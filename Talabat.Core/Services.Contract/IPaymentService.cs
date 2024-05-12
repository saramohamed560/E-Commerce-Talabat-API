using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Talabat.Core.Entities;
using Talabat.Core.Entities.OrderAggregate;

namespace Talabat.Core.Services.Contract
{
    public interface IPaymentService
    {
        //Create Or Update Payment intent 
        Task<CustomerBasket?> CreateOrUpdateIntent(string basketId);

        Task<Order> UpdatePaymentIntentToSuccessOrField(string paymentIntentId , bool flag);

    }
}
