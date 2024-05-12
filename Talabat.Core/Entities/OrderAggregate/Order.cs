using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Talabat.Core.Entities.OrderAggregate
{
	public class Order :BaseEntity
	{
        public Order()
        {
            
        }
		public Order(string buyerEmail, Address shippingAddress, DeliveryMethod deliveryMethod, ICollection<OrderItem> orderItems, decimal subTotal,string paymentIntentId)
		{
			BuyerEmail = buyerEmail;
			ShippingAddress = shippingAddress;
			DeliveryMethod = deliveryMethod;
			OrderItems = orderItems;
			SubTotal = subTotal;
            PaymentIntentId = paymentIntentId;
		}

		public string BuyerEmail { get; set; }
        public DateTimeOffset OrderDate { get; set; } = DateTimeOffset.UtcNow;
        public OrderStatus Status { get; set; } = OrderStatus.Pending;
        public Address ShippingAddress { get; set; } // Associassion
        //public int DeliveryMethodId { get; set; } //FK
        //EF Understand from Navigational Property
        public DeliveryMethod DeliveryMethod { get; set; }

        public ICollection<OrderItem> OrderItems { get; set; } = new HashSet<OrderItem>();

        public decimal SubTotal { get; set; }

        public decimal GetTotal()
            => SubTotal + DeliveryMethod.Cost;
        public string PaymentIntentId { get; set; } = string.Empty;



    }
}
