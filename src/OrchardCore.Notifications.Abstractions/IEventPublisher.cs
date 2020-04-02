using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;

namespace OrchardCore.Notifications
{
    public interface IEventPublisher
    {
        Task Publish<T>(T @event, [CallerMemberName]string eventName = default, CancellationToken cancellationToken = default) where T : class, IEvent;
    }
}
