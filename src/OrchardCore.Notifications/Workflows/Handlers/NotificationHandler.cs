using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using OrchardCore.Notifications.Workflows.Activities;
using OrchardCore.Workflows.Services;

namespace OrchardCore.Notifications.Workflows
{
    internal sealed class NotificationHandler : IEventHandler<Notification>
    {
        private readonly IWorkflowManager _workflowManager;
        private readonly ILogger<AzureHubHandler> _logger;

        public NotificationHandler(IWorkflowManager workflowManager, ILogger<AzureHubHandler> logger)
        {
            _workflowManager = workflowManager;
            _logger = logger;
        }

        public Task Handle(Notification notification, string eventName, CancellationToken cancellationToken)
        {
            var properties = new Dictionary<string, object>
            {
                { "Notification", new Dictionary<string, object>(notification) },
                { "EventName", eventName }
            };

            NotificationEvent.TryExtractWorkflowId(eventName, out string workflowId);

            return _workflowManager.TriggerEventAsync(NotificationEvent.EventName, properties, workflowId);
        }


    }
}
