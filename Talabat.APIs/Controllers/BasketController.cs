using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Talabat.APIs.DTOs;
using Talabat.APIs.Errors;
using Talabat.Core.Entities;
using Talabat.Core.Repositories.Contract;

namespace Talabat.APIs.Controllers
{
   
    public class BasketController : BaseApiController
    {
        private readonly IBasketRepository _basketRepository;
        private readonly IMapper _mapper;

        public BasketController(IBasketRepository basketRepository ,IMapper mapper)
        {
            _basketRepository = basketRepository;
            _mapper = mapper;
        }
        //Get or Recreate
        [HttpGet] //Get : /qpi/Basket/id?=
        public async Task<ActionResult<CustomerBasket>> GetBasket(string id)
        {
            var basket = await _basketRepository.GetCustomerAsync(id);
            
            return Ok(basket??new CustomerBasket(id));
        }

        [HttpPost]

        public async Task<ActionResult<CustomerBasket>> UpdateBasket(CustomerBasketDto basket)
        {
            var mappedCustomer = _mapper.Map<CustomerBasketDto, CustomerBasket>(basket);
            var createdOrUpdated = await _basketRepository.UpdateCustomerAsync(mappedCustomer);
            if (createdOrUpdated is null) return BadRequest(new ApiResponse(400));
            return Ok(createdOrUpdated);

        }

        [HttpDelete]
        public async Task DeleteBasket(string id)
        {
            await _basketRepository.DeleteCustomerAsync(id);
        }








    }
}
