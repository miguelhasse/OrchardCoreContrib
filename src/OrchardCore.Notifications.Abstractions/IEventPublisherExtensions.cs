using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;

namespace OrchardCore.Notifications
{
    public static class IEventPublisherExtensions
    {
        public static Task SendNotication(this IEventPublisher publisher, Notification notification, [CallerMemberName]string eventName = default, CancellationToken cancellationToken = default)
        {
            return publisher.Publish<Notification>(notification, eventName, cancellationToken);
        }
    }
}
