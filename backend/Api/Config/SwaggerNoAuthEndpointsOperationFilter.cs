using Microsoft.AspNetCore.Authorization;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace YouFoos.Api.Config
{
    /// <summary>
    /// This Swagger operation filter is used to only require Swagger UI authentication on endpoints
    /// that are protected - routes that do not require authentication will not show the lock.
    /// </summary>
    public class SwaggerNoAuthEndpointsOperationFilter : IOperationFilter
    {
        /// <summary>
        /// The main method of any Swagger operation filter.
        /// </summary>
        public void Apply(OpenApiOperation operation, OperationFilterContext context)
        {
            if (!DoesRouteRequireAuth(context.MethodInfo)) return;

            operation.Responses.TryAdd("401", new OpenApiResponse { Description = "Unauthorized" });
            operation.Responses.TryAdd("403", new OpenApiResponse { Description = "Forbidden" });

            var jwtbearerScheme = new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference 
                { 
                    Type = ReferenceType.SecurityScheme, 
                    Id = "bearer" 
                }
            };

            operation.Security = new List<OpenApiSecurityRequirement>
            {
                new OpenApiSecurityRequirement { [jwtbearerScheme] = System.Array.Empty<string>() }
            };
        }

        private static bool DoesRouteRequireAuth(MethodInfo context)
        {
            bool controllerHasAuthTag = context.DeclaringType.GetCustomAttributes(true).OfType<AuthorizeAttribute>().Any();
            bool routeHasAuthTag = context.GetCustomAttributes(true).OfType<AuthorizeAttribute>().Any();
            bool routeHasAllowAnonTag = context.GetCustomAttributes(true).OfType<AllowAnonymousAttribute>().Any();

            return (controllerHasAuthTag || routeHasAuthTag) && !routeHasAllowAnonTag;
        }
    }
}
