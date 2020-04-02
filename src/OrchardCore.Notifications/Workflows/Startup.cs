using Microsoft.Extensions.DependencyInjection;
using OrchardCore.Modules;
using OrchardCore.Notifications.Workflows.Activities;
using OrchardCore.Notifications.Workflows.Drivers;
using OrchardCore.Workflows.Helpers;
using OrchardCore.Workflows.Services;

namespace OrchardCore.Notifications.Workflows
{
    [Feature(Constants.Features.Activities)]
    [RequireFeatures(Constants.Features.Core, "OrchardCore.Workflows")]
    public class Startup : StartupBase
    {
        public override void ConfigureServices(IServiceCollection services)
        {
            services.AddActivity<NotificationEvent, NotificationEventDisplay>();
            services.AddActivity<NotificationTask, NotificationTaskDisplay>();

            services.AddScoped<IEventHandler<Notification>, NotificationHandler>();
        }
    }
}
