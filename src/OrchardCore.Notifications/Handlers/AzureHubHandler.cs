using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json.Linq;
using OrchardCore.Notifications.Hubs;

namespace OrchardCore.Notifications
{
    internal sealed class AzureHubHandler : IEventHandler<Notification>
    {
        private readonly IPushNotificationService _pushService;
        private readonly ILogger<AzureHubHandler> _logger;

        public AzureHubHandler(IPushNotificationService pushService, ILogger<AzureHubHandler> logger)
        {
            _pushService = pushService;
            _logger = logger;
        }

        public Task Handle(Notification notification, string eventName, CancellationToken cancellationToken)
        {
            return _pushService.SendNotification(notification.GetProperties(), null);
        }
    }
}
