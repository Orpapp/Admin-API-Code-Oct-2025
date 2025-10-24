using Microsoft.AspNetCore.Mvc.ApiExplorer;
using Microsoft.AspNetCore.Mvc.Controllers;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace Api.Helper
{
    public class SwaggerDefaultValues : IOperationFilter
    {
        public void Apply(OpenApiOperation operation, OperationFilterContext context)
        {
            var apiDescription = context.ApiDescription;
            operation.Deprecated |= apiDescription.IsDeprecated();

            foreach (var responseType in context.ApiDescription.SupportedResponseTypes)
            {
                var responseKey = responseType.IsDefaultResponse ? "default" : responseType.StatusCode.ToString();
                var response = operation.Responses[responseKey];

                foreach (var contentType in response.Content.Keys)
                {
                    if (responseType.ApiResponseFormats.All(x => x.MediaType != contentType))
                    {
                        response.Content.Remove(contentType);
                    }
                }
            }

            if (operation.Parameters == null)
            {
                return;
            }

            if (context.ApiDescription.ActionDescriptor is ControllerActionDescriptor descriptor && !descriptor.ControllerName.StartsWith("Account2321"))
            {
                operation.Parameters.Add(new OpenApiParameter()
                {
                    Name = "UtcOffsetInSecond",
                    In = ParameterLocation.Header,
                    Description = "Utc Offset In Seconds",
                    Required = true
                });
                operation.Parameters.Add(new OpenApiParameter()
                {
                    Name = "AccessToken",
                    In = ParameterLocation.Header,
                    Description = "Access Token",
                    Required = false
                });
                operation.Parameters.Add(new OpenApiParameter()
                {
                    Name = "AppVersion",
                    In = ParameterLocation.Header,
                    Description = "App Version",
                    Required = true
                });
            }
        }
    }
}
