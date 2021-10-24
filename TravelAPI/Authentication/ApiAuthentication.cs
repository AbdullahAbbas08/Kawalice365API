using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BalarinaAPI.Authentication
{
    struct JSON
    {
        public string message;
    }
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method)]
    public class ApiAuthentication : Attribute, IAsyncActionFilter
    {
        private const string ApiKeyHeaderName = "ApiKey";
        public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
        {
            JSON response;
            if(!context.HttpContext.Request.Headers.TryGetValue(ApiKeyHeaderName, out var potentialApiKey))
            {
                response.message = "401 Unauthorized";
                context.Result = new ObjectResult(response) { StatusCode = 401 };
                return;
            }
            var configuration = context.HttpContext.RequestServices.GetRequiredService<IConfiguration>();
            var apiKey = configuration.GetValue<string>("ApiKey");
            if(!apiKey.Equals(potentialApiKey))
            {
                response.message = "401 Unauthorized";
                context.Result = new ObjectResult(response) { StatusCode = 401 };
                return;
            }

            await next();
        }
    }
}
