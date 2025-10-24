using Microsoft.AspNetCore.Mvc.Controllers;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace Shared.Common
{
    public class AddHeader : IOperationFilter
    {
        public void Apply(OpenApiOperation operation, OperationFilterContext context)
        {
            if (operation.Parameters == null)
            {
                operation.Parameters = new List<OpenApiParameter>();
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
