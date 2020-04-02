using System.Collections.Generic;
using System.Threading.Tasks;

namespace OrchardCore.Notifications.Hubs
{
    public interface IPushNotificationService
    {
        Task<string> CreateRegistrationId(string handle);

        Task Register(MobilePlatform platform, string handle, string registrationId, IEnumerable<string> tags);

        Task Unregister(string registrationId);

        Task<string> SendNotification(IDictionary<string, string> properties, IEnumerable<string> tags);
    }

    public enum MobilePlatform
    {
        Windows, Apple, Android
    }
}
