using OrchardCore.Modules.Manifest;
using OrchardCore.Notifications;

[assembly: Module(
    Name = "Notifications",
    Author = "Miguel Hasse de Oliveira",
    Version = "0.1.0",
    Description = "Provides notifications service to broadcast events."
)]

[assembly: Feature(
    Id = Constants.Features.Core,
    Name = "Notification Events",
    Category = "Messaging",
    Description = "Registers the notifications core components."
)]

[assembly: Feature(
    Id = Constants.Features.SignalHub,
    Name = "SignalR Notifications",
    Category = "Messaging",
    Description = "Provides notification publishing service based on SignalR Hub and Azure SignalR Service.",
    Dependencies = new[] { Constants.Features.Core }
)]

[assembly: Feature(
    Id = Constants.Features.AzureHub,
    Name = "Azure Notification Hub",
    Category = "Messaging",
    Description = "Provides notification publishing service based on Azure Notification Hub.",
    Dependencies = new[] { Constants.Features.Core }
)]

[assembly: Feature(
    Id = Constants.Features.Activities,
    Name = "Notification Activities",
    Category = "Workflows",
    Description = "Provides notification services and activities.",
    Dependencies = new[] { Constants.Features.Core, "OrchardCore.Workflows" }
)]
