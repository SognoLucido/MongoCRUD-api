using Microsoft.AspNetCore.Authorization;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace Bookstore_backend
{
    public class AuthResponsesOperationFilter : IOperationFilter
    {
        public void Apply(OpenApiOperation operation, OperationFilterContext context)
        {
            var isAuthorized = (context.MethodInfo.DeclaringType.GetCustomAttributes(true).OfType<AuthorizeAttribute>().Any()
                        || context.MethodInfo.GetCustomAttributes(true).OfType<AuthorizeAttribute>().Any())
                        && !context.MethodInfo.GetCustomAttributes(true).OfType<AllowAnonymousAttribute>().Any();


            if (isAuthorized)
            {
                operation.Security.Add(new OpenApiSecurityRequirement
                 {
                     {
                         new OpenApiSecurityScheme
                         {
                                Reference = new OpenApiReference
                                {
                                    Type = ReferenceType.SecurityScheme,
                                    Id = "apikey"
                                }
                         },
                         Array.Empty<string>()
                     }
                 });

                //operation.Responses.Add("401", new OpenApiResponse { Description = "Unauthorized" });
            }

        }
    }
}
