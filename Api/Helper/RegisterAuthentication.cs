using Api.Authorization.JWT;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using Shared.Model.Base;
using Shared.Resources;
using System.Net;

namespace Api.Helper
{
    public static class RegisterAuthentication
    {
        public static void ConfigureJWTAuthentication(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
               .AddJwtBearer(options =>
               {
                   options.TokenValidationParameters = new TokenValidationParameters()
                   {
                       ValidateIssuer = true,
                       ValidateAudience = true,
                       ValidateIssuerSigningKey = true,
                       ValidateLifetime = true,
                       ValidIssuer = configuration.GetValue<string>("JwtTokenSettings:IsUser"),
                       ValidAudience = configuration.GetValue<string>("JwtTokenSettings:Audience"),
                       IssuerSigningKey = JwtSecurityKey.Create(configuration.GetValue<string>("JwtTokenSettings:Secret") ?? string.Empty),
                   };


                   options.Events = new JwtBearerEvents
                   {
                       OnAuthenticationFailed = context =>
                       {
                           context.NoResult();
                           context.Response.StatusCode = (int)HttpStatusCode.Unauthorized;
                           context.Response.ContentType = "application/json";
                           context.Response.WriteAsync(Newtonsoft.Json.JsonConvert.SerializeObject(
                               new ApiResponse<string> { Data = null, Message = ResourceString.UnauthorizedMessage }
                           , new JsonSerializerSettings
                           {
                               ContractResolver = new CamelCasePropertyNamesContractResolver()
                           })).Wait();
                           return Task.CompletedTask;
                       },
                       OnTokenValidated = context =>
                       {
                           return Task.CompletedTask;
                       }
                   };
               });
        }
    }
}
