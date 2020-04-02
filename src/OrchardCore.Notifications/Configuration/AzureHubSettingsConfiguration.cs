using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using OrchardCore.Environment.Shell;
using OrchardCore.Modules;

namespace OrchardCore.Notifications.Configuration
{
    [Feature(Constants.Features.AzureHub)]
    public class AzureHubSettingsConfiguration : IConfigureOptions<AzureHubSettings>
    {
        private readonly ShellSettings _shellSettings;

        public AzureHubSettingsConfiguration(ShellSettings shellSettings)
        {
            _shellSettings = shellSettings;
        }

        public void Configure(AzureHubSettings options) =>
            _shellSettings.ShellConfiguration.GetSection(Constants.Features.AzureHub).Bind(options);
    }
}
