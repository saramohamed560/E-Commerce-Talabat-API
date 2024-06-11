using AutoMapper;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Talabat.APIs.DTOs;
using Talabat.APIs.Errors;
using Talabat.APIs.Helpers;
using Talabat.Core;
using Talabat.Core.Entities;
using Talabat.Core.Repositories.Contract;
using Talabat.Core.Specifications.ProductsSpecs;

namespace Talabat.APIs.Controllers
{

    public class ProductsController : BaseApiController
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        public ProductsController(IUnitOfWork unitOfWork,
         IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        [Cached(600)] //action filter
        [HttpGet]
        //baseUrl/api/Products
          public async  Task<ActionResult<Pagination<ProductToReturnDTO>>> GetProducts([FromQuery]ProductSpecParams specParams){
            var spec = new ProductWithBrandAndCategorySpec(specParams);
            var Products = await _unitOfWork.Repository<Product>().GetAllWithSpecAsync(spec);
            var data = _mapper.Map<IReadOnlyList<Product>, IReadOnlyList<ProductToReturnDTO>>(Products);
            var countSpec = new ProductWithFiltarationForCountSpecifications(specParams);
            var count = await _unitOfWork.Repository<Product>().GetCountAsync(countSpec);
            return Ok(new Pagination<ProductToReturnDTO>(specParams.PageSize,specParams.PageIndex,count,data));
         }
        [Cached(600)]
        [HttpGet("{id}")]
        //BaseUrl/api/products/1
        //Imporove Swagger Documentation
        [ProducesResponseType(typeof(ProductToReturnDTO),200)]
        [ProducesResponseType(typeof(ApiResponse),404)]
        public async Task<ActionResult<ProductToReturnDTO>> GetProduct(int id)
        {
            var spec = new ProductWithBrandAndCategorySpec(id);

            var product = await _unitOfWork.Repository<Product>().GetEntityWithSpecAsync(spec);

            if (product is null)
                return BadRequest(new ApiResponse(404));//404
            return Ok(_mapper.Map<Product,ProductToReturnDTO>(product));//200
        }

        [Cached(600)]
        [HttpGet("brands")]// /api/Products/brands

        public async Task<ActionResult<IReadOnlyList<ProductBrand>>> GetBrands (){
            var brands = await _unitOfWork.Repository<ProductBrand>().GetAllAsync();
            return Ok(brands);
      
        }

        [Cached(600)]
        [HttpGet("categories")]// /api/Products/categories
        public async Task<ActionResult<IReadOnlyList<ProductCategory>>> GetCategories()
        {
            var categories = await _unitOfWork.Repository<ProductCategory>().GetAllAsync();
            return Ok(categories);
        }

    }

}
