using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using Talabat.APIs.DTOs;
using Talabat.APIs.Errors;
using Talabat.Core.Entities.OrderAggregate;
using Talabat.Service;

namespace Talabat.APIs.Controllers
{
    
    public class OrdersController : BaseApiController
    {
        private readonly IOrderService _orderService;
        private readonly IMapper _mapper;

        public OrdersController(IOrderService orderService ,IMapper mapper)
        {
            _orderService = orderService;
            _mapper = mapper;
        }
        //Create Order
        [HttpPost]
        [Authorize]
        [ProducesResponseType(typeof(Order), 200)]
        [ProducesResponseType(typeof(ApiResponse), 400)]
        public async Task<ActionResult<Order>> CreateOrder(OrderDto orderDto)
        {
            var buyerEmail = User.FindFirstValue(ClaimTypes.Email);
            var mappedAddress = _mapper.Map<AddressDto, Address>(orderDto.shipToAddress);
            var Order = await _orderService.CreateOrderAsync(buyerEmail, orderDto.BasketId, orderDto.DeliveryMethodId,mappedAddress);
            if (Order is null) return BadRequest(new ApiResponse(400, "There is A Problem in Your Order"));
            return Ok(Order);
        }

        [HttpGet]
        [Authorize]
        [ProducesResponseType(typeof(IReadOnlyList<Order>), 200)]
        [ProducesResponseType(typeof(ApiResponse), 404)]
        public async Task<ActionResult<IReadOnlyList<OrderToReturnDto>>> GetOrdersForUser()
        {
            var buyerEmail = User.FindFirstValue(ClaimTypes.Email);
            var orders=await _orderService.GetOrderForSpecificUserAsync(buyerEmail);
            if (orders is null) return NotFound(new ApiResponse(404, "There is no orders for this user"));
            var mappedOrder = _mapper.Map<IReadOnlyList<Order>, IReadOnlyList<OrderToReturnDto>>(orders);
            return Ok(mappedOrder);
        }
        [ProducesResponseType(typeof(Order), 200)]
        [ProducesResponseType(typeof(ApiResponse), 404)]
        [Authorize]
        [HttpGet("{id}")]
        public async Task<ActionResult<OrderToReturnDto>> GetOrderByIdForUser(int id)
        {
            var buyerEmail = User.FindFirstValue(ClaimTypes.Email);
            var order = await _orderService.GetOrderByIdForSpecificUserAsync(buyerEmail, id);
            if(order is null) return NotFound(new ApiResponse(404, $"There is no order with id {id} for this user"));
            var mappedOrder = _mapper.Map<Order, OrderToReturnDto>(order);
            return Ok(mappedOrder);
        }

        [HttpGet("DeliveryMethods")]
        public async Task<ActionResult<DeliveryMethod>> GetDeliveryMethods()
        {
            var DeliveryMethods = await _orderService.GetDeliveryMethodsAsync();
            return Ok(DeliveryMethods); 
        }
    }
}
