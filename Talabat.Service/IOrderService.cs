using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;
using Talabat.Core.Entities.OrderAggregate;

namespace Talabat.Service
{
    public  interface IOrderService
    {
        Task<Order?> CreateOrderAsync(string buyerEmail, string basketId, int deliveryMethodId, Address shippingAddress);

        Task<IReadOnlyList<Order>> GetOrderForSpecificUserAsync(string buyerEmail);

        Task<Order> GetOrderByIdForSpecificUserAsync(string buyerEmail, int orderId);
        Task<IReadOnlyList<DeliveryMethod>> GetDeliveryMethodsAsync();
        

    }
}
