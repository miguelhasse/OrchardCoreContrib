using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Localization;
using OrchardCore.Notifications;
using OrchardCore.Notifications.ViewModels;
using OrchardCore.Workflows.Abstractions.Models;
using OrchardCore.Workflows.Activities;
using OrchardCore.Workflows.Models;

namespace OrchardCore.Notifications.Workflows.Activities
{
    public class NotificationTask : TaskActivity
    {
        private readonly IEventPublisher _publisher;
        private readonly IStringLocalizer<NotificationTask> S;

        public NotificationTask(IEventPublisher publisher, IStringLocalizer<NotificationTask> localizer)
        {
            _publisher = publisher;
            S = localizer;
        }

        public static string TaskName => nameof(NotificationTask);

        public override string Name => TaskName;

        public override LocalizedString DisplayText => S["Notification Task"];

        public override LocalizedString Category => S["Notification"];

        public IEnumerable<NotificationProperty> Fields
        {
            get => GetProperty<IEnumerable<NotificationProperty>>();
            set => SetProperty(value);
        }

        public bool MergeInput
        {
            get => GetProperty(() => false);
            set => SetProperty(value);
        }

        public override IEnumerable<Outcome> GetPossibleOutcomes(WorkflowExecutionContext workflowContext, ActivityContext activityContext)
        {
            return Outcomes(S["Done"]);
        }

        public override async Task<ActivityExecutionResult> ExecuteAsync(WorkflowExecutionContext workflowContext, ActivityContext activityContext)
        {
            var dictionary = Fields?.ToDictionary(k => k.Name, v => (object)v.Value) ?? new Dictionary<string, object>();

            if (MergeInput)
            {
                var properties = workflowContext.Input["Notification"] as IDictionary<string, object>;
                dictionary = new Dictionary<string, object>(dictionary.Union(properties));
            }

            var notification = new Notification(dictionary);
            
            await _publisher.Publish(notification, string.Join(":", Name, workflowContext.WorkflowId));

            return Noop();
        }
    }
}
