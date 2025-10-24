using Shared.Common;
using Shared.Model.JWT;

namespace Api.Helper
{
    public static class RegisterModelSettings
    {
        public static void ConfigureModelSettings(this IServiceCollection services, IConfiguration configuration)
        {
            services.Configure<JwtTokenSettings>(configuration.GetSection("JwtTokenSettings"));//for jwt token
            services.Configure<SiteKeys>(configuration.GetSection("SiteKeys"));
            services.Configure<EmailConfigurationKeys>(configuration.GetSection("EmailConfigurationKeys"));
        }
    }
}
