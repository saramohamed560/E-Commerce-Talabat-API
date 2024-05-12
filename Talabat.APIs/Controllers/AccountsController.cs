using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using Talabat.APIs.DTOs;
using Talabat.APIs.Errors;
using Talabat.APIs.Extensions;
using Talabat.Core.Entities.Identity;
using Talabat.Core.Services.Contract;

namespace Talabat.APIs.Controllers
{

    public class AccountsController : BaseApiController
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly ITokenService _tokenService;
        private readonly IMapper _mapper;

        public AccountsController(UserManager<ApplicationUser> userManager,
            SignInManager<ApplicationUser> signInManager,
            ITokenService tokenService,
            IMapper mapper
            )
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _tokenService = tokenService;
            _mapper = mapper;
        }
        //Register
        [HttpPost("Register")]
        public async Task<ActionResult<ApplicationUserDto>> Register(RegisterDto model)
        {
            if (CheckEmailExist(model.Email).Result.Value)
            {
                return BadRequest(new ApiResponse(400, "This Email Already Exist"));
            }
            var user = new ApplicationUser()
            {
                Email = model.Email,
                UserName = model.Email.Split('@')[0],
                DisplayName = model.DisplayName,
                PhoneNumber = model.PhoneNumber
            };
            var result = await _userManager.CreateAsync(user, model.Password);
            if (!result.Succeeded) return BadRequest(new ApiResponse(400));
            var returnedUser = new ApplicationUserDto()
            {
                DisplayName = user.DisplayName,
                Email = user.Email,
                Token = await _tokenService.CreateTokenAsync(user, _userManager)
            };
            return Ok(returnedUser);
        }
       
        //Login
        [HttpPost("login")]
        public async Task<ActionResult<ApplicationUserDto>> LogIn(LoginDto model)
        {
            var user = await _userManager.FindByEmailAsync(model.Email);
            if (user is null) return Unauthorized(new ApiResponse(401));
            var result = await _signInManager.CheckPasswordSignInAsync(user, model.Password, false);
            if (!result.Succeeded) return Unauthorized(new ApiResponse(401));
            return Ok(new ApplicationUserDto()
            {
                DisplayName = user.DisplayName,
                Email = user.Email,
                Token = await _tokenService.CreateTokenAsync(user, _userManager)
            }) ;
        }
        
        [Authorize]
        [HttpGet("GetCurretUser")]
        public async Task<ActionResult<ApplicationUserDto>> GetCurretUser()
        {
            var email = User.FindFirstValue(ClaimTypes.Email);
            var user =  await _userManager.FindByEmailAsync(email);
            var returnedUser = new ApplicationUserDto {
                DisplayName = user.DisplayName,
                Email = user.Email,
                Token =  await _tokenService.CreateTokenAsync(user, _userManager)
            };
            return Ok(returnedUser);
        }

        [Authorize]
        [HttpGet("Address")]
        public async Task<ActionResult<AddressDto>> GetUserAddress()
        {
            //var email = User.FindFirstValue(ClaimTypes.Email);
            //var user = _userManager.FindByEmailAsync(email);
            //Address navigational property , FindByEmailAsync doesnot load navigational property
            var user = await _userManager.FindUserWithAddressAsync(User);
            var mappedAddress = _mapper.Map<Address,AddressDto>(user.Address);
            return Ok(mappedAddress);
        }

        [Authorize]
        [HttpPut("Address")]
        public async Task<ActionResult<AddressDto>> UpdateAddress(AddressDto addressDto)
        {
            var user = await _userManager.FindUserWithAddressAsync(User);
            var updatedAddress = _mapper.Map<AddressDto, Address>(addressDto);
            updatedAddress.Id=user.Address.Id ;
            user.Address = updatedAddress;
            var result = await _userManager.UpdateAsync(user);
            if (!result.Succeeded) return BadRequest(new ApiResponse(400));
            return Ok(addressDto); 
        }

        [HttpGet("emailExists")]
        public async Task<ActionResult<bool>> CheckEmailExist (string email)
        {
            //var user = _userManager.FindByEmailAsync(email);
            //if (user is null) return false;
            //return true;
            return  await _userManager.FindByEmailAsync(email) is not null;

        }
    }
}
