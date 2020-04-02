using System;
using Microsoft.Azure.NotificationHubs;
using OrchardCore.Notifications.Configuration;

namespace OrchardCore.Notifications.Hubs
{
    internal class AzureHubFactory : INotificationHubFactory
    {
        private readonly Lazy<NotificationHubClient> _notificationHubClient;

        public AzureHubFactory(AzureHubSettings settings)
        {
            _notificationHubClient = new Lazy<NotificationHubClient>(() =>
                NotificationHubClient.CreateClientFromConnectionString(settings.Connection, settings.Hub));
        }

        public NotificationHubClient NotificationHubClient => _notificationHubClient.Value;
    }
}
