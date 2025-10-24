using Hangfire;
using Hangfire.Dashboard;
using System.Diagnostics.CodeAnalysis;

namespace OrpWeb.Hangfire
{
    public class HangfireAuthorization : IDashboardAuthorizationFilter
    {
        public bool Authorize([NotNull] DashboardContext context)
        {
            var httpContext = context.GetHttpContext();

          
            return true;
         
        }
    }
}
