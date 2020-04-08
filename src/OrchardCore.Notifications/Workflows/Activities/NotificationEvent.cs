using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Extensions.Localization;
using OrchardCore.Notifications.ViewModels;
using OrchardCore.Workflows.Abstractions.Models;
using OrchardCore.Workflows.Activities;
using OrchardCore.Workflows.Models;

namespace OrchardCore.Notifications.Workflows.Activities
{
    public class NotificationEvent : EventActivity
    {
        private readonly IStringLocalizer<NotificationEvent> S;

        public NotificationEvent(IStringLocalizer<NotificationEvent> localizer)
        {
            S = localizer;
        }

        public static string EventName => nameof(NotificationEvent);

        public override string Name => EventName;

        public override LocalizedString DisplayText => S["Notification Event"];

        public override LocalizedString Category => S["Notification"];

        public IEnumerable<NotificationProperty> Filters
        {
            get => GetProperty<IEnumerable<NotificationProperty>>();
            set => SetProperty(value);
        }

        public override IEnumerable<Outcome> GetPossibleOutcomes(WorkflowExecutionContext workflowContext, ActivityContext activityContext)
        {
            return Outcomes(S["Done"]);
        }

        public override bool CanExecute(WorkflowExecutionContext workflowContext, ActivityContext activityContext)
        {
            if (workflowContext.Input["EventName"] is string eventName && TryExtractWorkflowId(eventName, out var workflowId)
                && workflowContext.CorrelationId == workflowId)
                return false;
            
            var properties = workflowContext.Input["Notification"] as IDictionary<string, object>;

            return Filters?.Any(prop => properties[prop.Name]?.ToString() != prop.Value) ?? false == false;
        }

        public override ActivityExecutionResult Resume(WorkflowExecutionContext workflowContext, ActivityContext activityContext)
        {
            return Outcomes("Done");
        }

        internal static bool TryExtractWorkflowId(string eventName, out string workflowId)
        {
            var parts = eventName.Split(':');

            if (parts.Length >= 2 && parts[0].Equals(NotificationTask.TaskName, StringComparison.OrdinalIgnoreCase))
            {
                workflowId = parts[1];
                return true;
            }

            workflowId = null;
            return false;
        }
    }
}
