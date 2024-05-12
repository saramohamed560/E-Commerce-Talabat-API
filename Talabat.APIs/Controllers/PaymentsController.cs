using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Stripe;
using Talabat.APIs.DTOs;
using Talabat.APIs.Errors;
using Talabat.Core.Entities;
using Talabat.Core.Services.Contract;

namespace Talabat.APIs.Controllers
{
    
    public class PaymentsController : BaseApiController
    {
        private readonly IPaymentService _paymentService;
        private readonly IMapper _mapper;
        const string endpointSecret = "whsec_51be07f34ed9be2d90e862ca3c42436f651e994073c842eb6ac4dda02e07cdbb";

        public PaymentsController(IPaymentService paymentService, IMapper mapper)
        {
            _paymentService = paymentService;
            _mapper = mapper;
        }
        [Authorize]
        [ProducesResponseType(typeof(CustomerBasketDto), 200)]
        [ProducesResponseType(typeof(ApiResponse), 400)]
        [HttpPost("{BasketId}")]
        public async Task<ActionResult<CustomerBasketDto>> CreateOrUpdateBasket(string BasketId)
        {
            var CustomerBasket = await _paymentService.CreateOrUpdateIntent(BasketId);
            if (CustomerBasket is null) return BadRequest(new ApiResponse(400, "Has An Problem With Your Basket"));
            var MappedBasket = _mapper.Map<CustomerBasket, CustomerBasketDto>(CustomerBasket);
            return Ok(MappedBasket);

        }
        [HttpPost("webhook")] //Post : baseUrl/api/Payments/webhook
        public async Task<IActionResult> StripeWebhook()
        {
            var json = await new StreamReader(HttpContext.Request.Body).ReadToEndAsync();
            try
            {
                var stripeEvent = EventUtility.ConstructEvent(json,
                    Request.Headers["Stripe-Signature"], endpointSecret);
                var paymentIntent = stripeEvent.Data.Object as PaymentIntent;

                // Handle the event
                if (stripeEvent.Type == Events.PaymentIntentPaymentFailed)
                {
                    await  _paymentService.UpdatePaymentIntentToSuccessOrField(paymentIntent.Id,false);
                }
                else if (stripeEvent.Type == Events.PaymentIntentSucceeded)
                {
                    await _paymentService.UpdatePaymentIntentToSuccessOrField(paymentIntent.Id, false);
                }
                return Ok();
            }
            catch (StripeException e)
            {
                return BadRequest();
            }
        }

    } 
}
