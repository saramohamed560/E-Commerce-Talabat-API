using Microsoft.AspNetCore.Builder;

namespace Talabat.APIs.Extensions
{
    public  static class SwaggerServices
    {
        public static IServiceCollection AddSwaggerServices(this IServiceCollection services)
        {
            services.AddEndpointsApiExplorer();
            services.AddSwaggerGen();
            return services;
        }

        public static WebApplication  UseSwaggerMiddleWares(this WebApplication builder)
        {
            builder.UseSwagger();
            builder.UseSwaggerUI();
            return builder;

        }
    }
}
