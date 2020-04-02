using System;
using System.Collections.Immutable;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Localization;
using OrchardCore.Environment.Shell.Descriptor.Models;
using OrchardCore.Navigation;
using OrchardCore.Notifications.Drivers;

namespace OrchardCore.Notifications
{
    public class AdminMenu : INavigationProvider
    {
        private readonly ShellDescriptor _shellDescriptor;
        private readonly IStringLocalizer<AdminMenu> S;

        public AdminMenu(IStringLocalizer<AdminMenu> localizer, ShellDescriptor shellDescriptor)
        {
            S = localizer;
            _shellDescriptor = shellDescriptor;
        }

        public Task BuildNavigationAsync(string name, NavigationBuilder builder)
        {
            if (!String.Equals(name, "admin", StringComparison.OrdinalIgnoreCase))
                return Task.CompletedTask;

            builder
                .Add(S["Configuration"], configuration => configuration
                    .Add(S["Settings"], settings =>
                    {
                        var features = _shellDescriptor.Features.Select(feature => feature.Id).ToImmutableArray();

                        bool azureHubEnabled = features.Contains(Constants.Features.AzureHub);
                        bool signalHubEnabled = features.Contains(Constants.Features.SignalHub);
                        int position = 0;

                        if (azureHubEnabled || signalHubEnabled)
                        {
                            settings.Add(S["Notifications"], "5", notifications =>
                            {
                                if (azureHubEnabled)
                                {
                                    notifications.Add(S["Push Hubs"], (++position).ToString(), entry => entry
                                      .Action("Index", "Admin", new { area = "OrchardCore.Settings", groupId = AzureHubSettingsDisplayDriver.GroupId })
                                      .Permission(Permissions.ManageNotificationSettings)
                                      .LocalNav());
                                }

                                if (signalHubEnabled)
                                {
                                    notifications.Add(S["SignalR Hub"], (++position).ToString(), entry => entry
                                      .Action("Index", "Admin", new { area = "OrchardCore.Settings", groupId = SignalHubSettingsDisplayDriver.GroupId })
                                      .Permission(Permissions.ManageNotificationSettings)
                                      .LocalNav());
                                }
                            });
                        }
                    }));

            return Task.CompletedTask;
        }
    }
}
