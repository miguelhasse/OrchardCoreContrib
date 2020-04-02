using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using OrchardCore.Environment.Shell;
using OrchardCore.Modules;

namespace OrchardCore.Notifications.Configuration
{
    [Feature(Constants.Features.AzureHub)]
    public class SignalHubSettingsConfiguration : IConfigureOptions<SignalHubSettings>
    {
        private readonly ShellSettings _shellSettings;

        public SignalHubSettingsConfiguration(ShellSettings shellSettings)
        {
            _shellSettings = shellSettings;
        }

        public void Configure(SignalHubSettings options) =>
            _shellSettings.ShellConfiguration.GetSection(Constants.Features.SignalHub).Bind(options);
    }
}
