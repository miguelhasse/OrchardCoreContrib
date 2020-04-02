using System.Collections.Generic;
using System.Linq;
using OrchardCore.Notifications.ViewModels;
using OrchardCore.Notifications.Workflows.Activities;
using OrchardCore.Notifications.Workflows.ViewModels;
using OrchardCore.Workflows.Display;

namespace OrchardCore.Notifications.Workflows.Drivers
{
    public class NotificationTaskDisplay : ActivityDisplayDriver<NotificationTask, NotificationTaskViewModel>
    {
        protected override void EditActivity(NotificationTask source, NotificationTaskViewModel model)
        {
            model.Properties = source.Fields?.ToList() ?? new List<NotificationProperty>();
            model.MergeInput = source.MergeInput;
        }

        protected override void UpdateActivity(NotificationTaskViewModel model, NotificationTask target)
        {
            target.Fields = model.Properties;
            target.MergeInput = model.MergeInput;
        }
    }
}
