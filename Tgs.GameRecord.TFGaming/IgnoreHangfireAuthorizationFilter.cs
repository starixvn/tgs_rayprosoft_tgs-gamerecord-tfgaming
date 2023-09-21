using Hangfire.Dashboard;

namespace Tgs.GameRecord.TFGaming;

public class IgnoreHangfireAuthorizationFilter : IDashboardAuthorizationFilter
{
    public bool Authorize(DashboardContext context) => true;
}
