using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Logging;
using OrchardCore.Notifications.Hubs;

namespace OrchardCore.Notifications
{
    internal sealed class SignalHubHandler : IEventHandler<Notification>
    {
        private readonly IHubContext<LocalHub> _hubContext;
        private readonly ILogger<AzureHubHandler> _logger;

        public SignalHubHandler(IHubContext<LocalHub> hubContext, ILogger<AzureHubHandler> logger)
        {
            _hubContext = hubContext;
            _logger = logger;
        }

        public Task Handle(Notification notification, string eventName, CancellationToken cancellationToken)
        {
            return _hubContext.Clients.All.SendAsync(LocalHub.ClientMethod, notification, cancellationToken);
        }
    }
}
