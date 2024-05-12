using System.Net;
using System.Text.Json;
using Talabat.APIs.Errors;

namespace Talabat.APIs.Middlewares
{
    //create MiddleWare By Convension
    //1.class must be ended with Middleware
    //2. must contain async func called invokeasync and take one param of type httpContext

    public class ExceptionMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger _logger;
        private readonly IHostEnvironment _env;

        public ExceptionMiddleware(RequestDelegate next ,ILogger<ExceptionMiddleware> logger,IHostEnvironment env)
        {
            _next = next;
            _logger = logger;
            _env = env;
        }

        public async Task InvokeAsync(HttpContext httpContext)
        {
            try
            {
                //Take her param with request
               await _next.Invoke(httpContext);
                //Take her param with response

            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message); // in development Env 
                //in case of production => log in (database| files)

                httpContext.Response.StatusCode = (int)HttpStatusCode.InternalServerError;//500
                httpContext.Response.ContentType = "application/json";

                var response=_env.IsDevelopment()?
                    new ApiExceptionResponse((int)HttpStatusCode.InternalServerError ,ex.Message,ex.StackTrace)
                    :new ApiExceptionResponse((int)HttpStatusCode.InternalServerError);
                var json = JsonSerializer.Serialize(response);
                var option = new JsonSerializerOptions()
                {
                    PropertyNamingPolicy = JsonNamingPolicy.CamelCase
                };

                await httpContext.Response.WriteAsync(json);

            }
        }

    }
}
