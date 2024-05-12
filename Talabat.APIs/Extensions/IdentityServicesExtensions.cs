using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using Talabat.Core.Entities.Identity;
using Talabat.Core.Services.Contract;
using Talabat.Repositotry._Identity;
using Talabat.Service;

namespace Talabat.APIs.Extensions
{
    public static class IdentityServicesExtensions
    {

        public static IServiceCollection AddIdentityServices(this IServiceCollection Services, IConfiguration configuration) {


            Services.AddIdentity<ApplicationUser, IdentityRole>()
                    .AddEntityFrameworkStores<ApplicationIdentityDbContext>()
                    ;
            Services.AddAuthentication(Options =>
            {
                Options.DefaultAuthenticateScheme=JwtBearerDefaults.AuthenticationScheme;
                Options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            })
                .AddJwtBearer(Options =>
                {
                    Options.TokenValidationParameters = new TokenValidationParameters()
                    {
                        ValidateIssuer = true,
                        ValidIssuer = configuration["JWT:ValidIssure"],
                        ValidateAudience = true,
                        ValidAudience = configuration["JWT:ValidAudience"],
                        ValidateLifetime = true,
                        ValidateIssuerSigningKey = true,
                        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(configuration["JWT:Key"]))
                    };
                }
            ) ;
                
            Services.AddScoped<ITokenService, TokenService>();


            return Services;


        }
    }
}
