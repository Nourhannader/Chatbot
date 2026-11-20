using Hangfire.Annotations;
using Hangfire.Dashboard;

namespace chatbot.Api.BackgroundJobs
{
    public sealed class HangfireDashboardAuthorizationFilter : IDashboardAuthorizationFilter
    {
        public bool Authorize([NotNull] DashboardContext context)
        {
            var httpContext = context.GetHttpContext();

            return httpContext.User.Identity?.IsAuthenticated == true
                   && httpContext.User.IsInRole("Admin");
        }

        
    }
}
