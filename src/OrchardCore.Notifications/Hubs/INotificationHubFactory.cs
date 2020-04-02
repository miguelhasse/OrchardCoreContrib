using Microsoft.Azure.NotificationHubs;

namespace OrchardCore.Notifications.Hubs
{
    public interface INotificationHubFactory
    {
        public NotificationHubClient NotificationHubClient { get; }
    }
}
