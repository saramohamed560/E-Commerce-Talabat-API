using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using Talabat.Core.Services.Contract;

namespace Talabat.APIs.Helpers
{
    public class CachedAttribute : Attribute, IAsyncActionFilter
    {
        private readonly int _timeToLiveInSec;

        public CachedAttribute(int timeToLiveInSec)
        {
            _timeToLiveInSec = timeToLiveInSec;
        }
        public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
        {
          var responseCacheService= context.HttpContext.RequestServices.GetRequiredService<IResponseCacheService>();
            //Ask CLR to create object from response cache service explicitly
            var cacheKey = GenerateCacheKeyFromRequest(context.HttpContext.Request);
            var response = await responseCacheService.GetCachedResponseAsync(cacheKey);
            //in case response already cached
            if (!response.IsNullOrEmpty())
            {
                var result = new ContentResult()
                {
                    Content = response,
                    ContentType = "aplication/json",
                    StatusCode = 200
                };
                context.Result = result;
                return;
            }
            var excutedActionContext= await next.Invoke(); //will excute next action filter if exist or excute action itself
            if(excutedActionContext.Result is OkObjectResult okObjectResult && okObjectResult.Value is not null)
            {
                responseCacheService.CacheResponseAsync(cacheKey, okObjectResult.Value, TimeSpan.FromSeconds(_timeToLiveInSec));
            }

        }

        private string GenerateCacheKeyFromRequest(HttpRequest request)
        {
            var keyBuilder = new StringBuilder();
            keyBuilder.Append(request.Path);// api/products
            //foreach on  querystring
            foreach(var (key,value) in request.Query.OrderBy(x=>x.Key))
            {
                keyBuilder.Append($"|{key}-{value}");
            }
            return keyBuilder.ToString();

        }
    }
}
