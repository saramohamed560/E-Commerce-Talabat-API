using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using Talabat.Core.Entities.Identity;
using Talabat.Core.Services.Contract;

namespace Talabat.Service
{
    public class TokenService : ITokenService
    {
        private readonly IConfiguration configuration;

        public TokenService(IConfiguration configuration)
        {
            this.configuration = configuration;
        }
        public async Task<string> CreateTokenAsync(ApplicationUser user ,UserManager<ApplicationUser>userManager)
        {
            //Payload
            //1.Private Payload (User-Defined)

            var AuthClaims = new List<Claim>() { 
             new Claim(ClaimTypes.Name,user.DisplayName),
             new Claim(ClaimTypes.Email,user.Email)
            };
            //Roles of user
            var roles = await userManager.GetRolesAsync(user);
            foreach (var role in roles)
            {
                AuthClaims.Add(new Claim(ClaimTypes.Role, role));
            }

            //Key : Using Symmetric SecrityKey
            var AuthKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(configuration["JWT:Key"]));

            //Register Claims
            //Issure : source who create key
            //Asdience 
            //ExpirationTime

            var token = new JwtSecurityToken(
                issuer: configuration["JWT:ValidIssure"],
                audience: configuration["JWT:ValidAudience"],
                expires:DateTime.Now.AddDays(double.Parse(configuration["JWT:DurationInDays"])),
                claims:AuthClaims,
                signingCredentials:new SigningCredentials(AuthKey,SecurityAlgorithms.HmacSha256Signature)
                );
                      
             //Generate token
             return  new JwtSecurityTokenHandler().WriteToken(token);



        }
    }
}
