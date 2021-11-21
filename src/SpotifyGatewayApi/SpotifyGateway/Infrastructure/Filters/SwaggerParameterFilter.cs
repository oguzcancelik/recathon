using System.Linq;
using Microsoft.AspNetCore.Mvc;
using Microsoft.OpenApi.Models;
using SpotifyGateway.Infrastructure.Attributes;
using SpotifyGateway.Infrastructure.Constants;
using SpotifyGateway.Infrastructure.Extensions;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace SpotifyGateway.Infrastructure.Filters
{
    public class SwaggerParameterFilter : IOperationFilter
    {
        public void Apply(OpenApiOperation operation, OperationFilterContext context)
        {
            var addAuthTokenHeader = context.ApiDescription.ActionDescriptor.EndpointMetadata.Any(
                x => x.GetType() == typeof(ServiceFilterAttribute) && ((ServiceFilterAttribute)x).ServiceType == typeof(UserAuthenticationAttribute)
            );

            if (addAuthTokenHeader)
            {
                var parameter = new OpenApiParameter
                {
                    Name = AuthConstants.AuthToken,
                    In = ParameterLocation.Header,
                    Required = false
                };

                operation.Parameters.Add(parameter);
            }

            var addAdminTokenCookie = RouteConstants.AdminRoutes.Any(x => x.EqualsIgnoreCase($"/{context.ApiDescription.RelativePath}"));

            if (addAdminTokenCookie)
            {
                var parameter = new OpenApiParameter
                {
                    Name = AuthConstants.AdminToken,
                    In = ParameterLocation.Cookie,
                    Required = false
                };

                operation.Parameters.Add(parameter);
            }

            var languageHeader = new OpenApiParameter
            {
                Name = HttpConstants.Language,
                In = ParameterLocation.Header,
                Required = false
            };

            operation.Parameters.Add(languageHeader);
        }
    }
}