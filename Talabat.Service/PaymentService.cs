using Microsoft.Extensions.Configuration;
using Stripe;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Talabat.Core;
using Talabat.Core.Entities;
using Talabat.Core.Entities.OrderAggregate;
using Talabat.Core.Repositories.Contract;
using Talabat.Core.Services.Contract;
using Talabat.Core.Specifications.OrderSpecs;
using Product = Talabat.Core.Entities.Product;

namespace Talabat.Service
{
    public class PaymentService : IPaymentService
    {
        private readonly IConfiguration _configuration;
        private readonly IBasketRepository _basketRepository;
        private readonly IUnitOfWork _unitOfWork;

        public PaymentService(IConfiguration configuration ,
            IBasketRepository basketRepository ,
            IUnitOfWork unitOfWork
            )
        {
            _configuration = configuration;
            _basketRepository = basketRepository;
            _unitOfWork = unitOfWork;
        }
        public async Task<CustomerBasket?> CreateOrUpdateIntent(string basketId)
        {
            //SecretKey
             StripeConfiguration.ApiKey = _configuration["StripeKeys:Secretkey"];
            //GetBasket
            var Basket = await _basketRepository.GetCustomerAsync(basketId);
            if (Basket is null) return null;
            var ShippingPrice=0M;
            if (Basket.DeliveryMethodId.HasValue)
            {
                var DeliveryMethod = await _unitOfWork.Repository<DeliveryMethod>().GetAsync(Basket.DeliveryMethodId.Value);
                ShippingPrice = DeliveryMethod.Cost;
            }
            //total =subtotal+DMCost
            if (Basket.Items.Count > 0)
            {
                foreach (var item in Basket.Items)
                {
                    var product =  await _unitOfWork.Repository<Product>().GetAsync(item.Id);
                    if (item.Price != product.Price)
                        item.Price = product.Price;
                }
            }
            var SubTotal = Basket.Items.Sum(item => item.Quantity * item.Price);
            //Create PaymentIntent 
            var Service = new PaymentIntentService();
            PaymentIntent paymentIntent;
            if (String.IsNullOrEmpty(Basket.PaymentIntentId))//Create
            {
                var options = new PaymentIntentCreateOptions()
                {
                    Amount=(long)(SubTotal*100+ShippingPrice*100),
                    Currency="usd",
                    PaymentMethodTypes=new List<string>() { "card"}
                };
                paymentIntent = await Service.CreateAsync(options);
                Basket.PaymentIntentId = paymentIntent.Id;
                Basket.ClientSecret = paymentIntent.ClientSecret;
            }
            else //Update
            {
                var options = new PaymentIntentUpdateOptions()
                {
                    Amount = (long)(SubTotal * 100 + ShippingPrice * 100),
                };
                paymentIntent = await Service.UpdateAsync(Basket.PaymentIntentId, options);
                Basket.PaymentIntentId = paymentIntent.Id;
                Basket.ClientSecret = paymentIntent.ClientSecret;
            }
            await _basketRepository.UpdateCustomerAsync(Basket);
            return Basket; 
        }

        public async Task<Order> UpdatePaymentIntentToSuccessOrField(string paymentIntentId, bool flag)
        {
            var spec = new OrderWithPaymentIntentSpec(paymentIntentId);
            var order = await _unitOfWork.Repository<Order>().GetEntityWithSpecAsync(spec);
            if (flag)
            {
                order.Status = OrderStatus.PaymentReceived;
            }
            else
            {
                order.Status = OrderStatus.PaymentFaild;

            }
            _unitOfWork.Repository<Order>().Update(order);
            await _unitOfWork.CompleteAsync();
            return order;
        }
    }
}
