using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Talabat.Core;
using Talabat.Core.Entities;
using Talabat.Core.Entities.OrderAggregate;
using Talabat.Core.Repositories.Contract;
using Talabat.Core.Services.Contract;
using Talabat.Core.Specifications.OrderSpecs;

namespace Talabat.Service
{
    public  class OrderService : IOrderService
    {
        private readonly IBasketRepository _basketRepository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IPaymentService _paymentService;

        public OrderService(IBasketRepository basketRepository, 
           IUnitOfWork unitOfWork ,IPaymentService paymentService
            )
        {
            _basketRepository = basketRepository;
            _unitOfWork = unitOfWork;
            _paymentService = paymentService;
        }
        public async Task<Order?> CreateOrderAsync(string buyerEmail, string basketId, int deliveryMethodId, Address shippingAddress)
        {
            //1.Get Basket from Basket Repo
            var Basket =await  _basketRepository.GetCustomerAsync(basketId);

            //2.Get Selected Items At Basket From Product  Repo
            var OrderItems = new List<OrderItem>();
            if(Basket?.Items?.Count > 0)
            {
                foreach (var item in Basket.Items)
                {
                    var product = await _unitOfWork.Repository<Product>().GetAsync(item.Id);
                    var productItemOrdered = new ProductItemOrdered(product.Id, product.Name, product.PictureUrl);
                    var orderItem = new OrderItem(productItemOrdered, product.Price, item.Quantity);
                    OrderItems.Add(orderItem);
                }
            }
            //3.Calculate SubTotal
            var subTotal = OrderItems.Sum(item => item.Price * item.Quantity);
            //4.Get Delivery Method From Delivery Method Repo
            var deliveryMethod = await _unitOfWork.Repository<DeliveryMethod>().GetAsync(deliveryMethodId);
            //5.Create Order 
            var spec = new OrderWithPaymentIntentSpec(Basket.PaymentIntentId);
            var ExOrder = await _unitOfWork.Repository<Order>().GetEntityWithSpecAsync(spec);
            if(ExOrder is not null)
            {
                _unitOfWork.Repository<Order>().Delete(ExOrder);
               await  _paymentService.CreateOrUpdateIntent(basketId);

            }

            var order = new Order(buyerEmail, shippingAddress, deliveryMethod, OrderItems, subTotal,Basket.PaymentIntentId);
            //6.Add Order Lacally 
            await _unitOfWork.Repository<Order>().AddAsync(order);
            //7. Save Order To DB (ToDo)
            var result = await _unitOfWork.CompleteAsync();
            if (result <= 0) return null;
            return order;
        }
        public async Task<IReadOnlyList<DeliveryMethod>> GetDeliveryMethodsAsync()
        {
            var DeliveryMethods = await _unitOfWork.Repository<DeliveryMethod>().GetAllAsync();
            return DeliveryMethods;
        }
        public async Task<Order> GetOrderByIdForSpecificUserAsync(string buyerEmail, int orderId)
        {
            var spec = new OrderSpecification(buyerEmail,orderId);
            var order=await _unitOfWork.Repository<Order>().GetEntityWithSpecAsync(spec);
            return order;
        }

        public  async Task<IReadOnlyList<Order>> GetOrderForSpecificUserAsync(string buyerEmail)
        {
            var spec = new OrderSpecification(buyerEmail);
            var orders =  await _unitOfWork.Repository<Order>().GetAllWithSpecAsync(spec);
            return orders;
        }
    }
}
