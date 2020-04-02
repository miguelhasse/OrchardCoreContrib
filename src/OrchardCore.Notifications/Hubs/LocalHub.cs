using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Logging;
using OrchardCore.Environment.Shell;

// https://stackoverflow.com/questions/31527034/signalr-multitenant-dependency-injection
// https://docs.microsoft.com/en-us/aspnet/signalr/overview/guide-to-the-api/mapping-users-to-connections

namespace OrchardCore.Notifications.Hubs
{
    internal class LocalHub : Hub
    {
        private readonly ShellSettings _settings;
        private readonly ILogger<LocalHub> _logger;

        internal const string ClientMethod = "event";

        public LocalHub(ShellSettings shellSettings, ILogger<LocalHub> logger)
        {
            _settings = shellSettings;
            _logger = logger;
        }

        public override Task OnDisconnectedAsync(Exception exception)
        {
            _logger.LogError(exception, "Local notification hub disconnected.");
            return Task.CompletedTask;
        }

        public Task Broadcast(Notification notification, CancellationToken cancellationToken = default) =>
            Clients.All.SendAsync(ClientMethod, notification, cancellationToken);

        public Task BroadcastToGroup(string groupName, Notification notification, CancellationToken cancellationToken = default) =>
            Clients.Group(groupName).SendAsync(ClientMethod, notification, cancellationToken);

        public Task SendToConnection(string connectionId, Notification notification, CancellationToken cancellationToken = default) =>
            Clients.Client(connectionId).SendAsync(ClientMethod, notification, cancellationToken);

        public Task JoinGroup(string groupName, CancellationToken cancellationToken = default) =>
            Groups.AddToGroupAsync(Context.ConnectionId, groupName, cancellationToken);

        public Task LeaveGroup(string groupName, CancellationToken cancellationToken = default) =>
            Groups.RemoveFromGroupAsync(Context.ConnectionId, groupName, cancellationToken);
    }
}
