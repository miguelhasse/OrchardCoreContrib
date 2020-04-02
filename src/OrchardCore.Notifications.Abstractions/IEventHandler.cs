using System.Threading;
using System.Threading.Tasks;

namespace OrchardCore.Notifications
{
    public interface IEventHandler<in T> where T : IEvent
    {
		Task Handle(T @event, string eventName, CancellationToken cancellationToken);
    }
}
