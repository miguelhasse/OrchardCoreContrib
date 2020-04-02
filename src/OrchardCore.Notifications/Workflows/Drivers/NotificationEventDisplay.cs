using System.Collections.Generic;
using System.Linq;
using OrchardCore.Notifications.ViewModels;
using OrchardCore.Notifications.Workflows.Activities;
using OrchardCore.Notifications.Workflows.ViewModels;
using OrchardCore.Workflows.Display;

namespace OrchardCore.Notifications.Workflows.Drivers
{
    public class NotificationEventDisplay : ActivityDisplayDriver<NotificationEvent, NotificationEventViewModel>
    {
        protected override void EditActivity(NotificationEvent source, NotificationEventViewModel model)
        {
            model.Properties = source.Filters?.ToList() ?? new List<NotificationProperty>();
        }

        protected override void UpdateActivity(NotificationEventViewModel model, NotificationEvent target)
        {
            target.Filters = model.Properties;
        }
    }
}
