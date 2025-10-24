using Business.Communication;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace IOC.Extensions
{
    public static class ContainerExtension
    {
        public static void RegisterWebApi(this IServiceCollection services, IConfiguration configuration)
        {
            services.RegisterServices(configuration);
            services.AddTransient<INotificationService, NotificationService>();
            services.AddTransient<IEmailFunctions, EmailFunctions>();
            services.AddTransient<IEmailHelperCore, EmailHelperCore>();
            services.AddTransient<IHttpContextAccessor, HttpContextAccessor>();
        }

    }
}
