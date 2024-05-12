using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Mvc;
using Talabat.APIs.Errors;
using Talabat.APIs.Helpers;
using Talabat.Core;
using Talabat.Core.Repositories.Contract;
using Talabat.Core.Services.Contract;
using Talabat.Repositotry;
using Talabat.Service;

namespace Talabat.APIs.Extensions
{
    public  static class ApplicationServicesExtensions
    {

        public static IServiceCollection AddApplicationServices(this IServiceCollection services)
        {
            services.AddScoped<IPaymentService, PaymentService>();
            services.AddScoped<IUnitOfWork, UnitOfWork>();
            services.AddScoped<IOrderService, OrderService>();
            services.AddScoped(typeof(IBasketRepository), typeof(BasketRepository));
            //webApplicationBuilder.Services.AddAutoMapper(M => M.AddProfile(new MappingProfile()));
            services.AddAutoMapper(typeof(MappingProfile));
            services.Configure<ApiBehaviorOptions>(options =>
            {
                //Factory:responsible of generating response of validation error
                options.InvalidModelStateResponseFactory = (actionContext) =>
                {
                    //ActionContext=>Context That Contain Action Has Errors
                    //ModelState =>Dictionary 
                    //Key:Param   ,  Value : Errors

                    var errors = actionContext.ModelState.Where(P => P.Value.Errors.Count() > 0)
                                              .SelectMany(P => P.Value.Errors)
                                              .Select(E => E.ErrorMessage)
                                              .ToList();
                    var response = new ValidationErrorResponse()
                    {
                        Errors = errors
                    };

                    return new BadRequestObjectResult(response);
                };

            });


            return services;
        }
    }
}
