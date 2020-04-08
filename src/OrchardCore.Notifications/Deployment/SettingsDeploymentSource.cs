using System.Threading.Tasks;
using Microsoft.Extensions.Options;
using Newtonsoft.Json.Linq;
using OrchardCore.Deployment;
using OrchardCore.Notifications.Configuration;

namespace OrchardCore.Notifications.Deployment
{
    public class SettingsDeploymentSource : IDeploymentSource
    {
        private readonly IConfigureOptions<AzureHubSettings> _azureHubOptions;
        private readonly IConfigureOptions<SignalHubSettings> _signalHubOptions;

        public SettingsDeploymentSource(
            IConfigureOptions<AzureHubSettings> azureHubOptions,
            IConfigureOptions<SignalHubSettings> signalHubOptions)
        {
            _azureHubOptions = azureHubOptions;
            _signalHubOptions = signalHubOptions;
        }

        public Task ProcessDeploymentStepAsync(DeploymentStep step, DeploymentPlanResult result)
        {
            var settingsStep = step as SettingsDeploymentStep;

            if (settingsStep == null || (!settingsStep.IncludeAzureHubSettings && !settingsStep.IncludeSignalHubSettings))
            {
                return Task.CompletedTask;
            }

            if (settingsStep.IncludeAzureHubSettings)
            {
                var options = new AzureHubSettings();
                _azureHubOptions.Configure(options);

                var data = new JArray();
                result.Steps.Add(new JObject(
                    new JProperty("name", nameof(AzureHubSettings)),
                    new JProperty("settings", JObject.FromObject(options))
                ));
            }

            if (settingsStep.IncludeSignalHubSettings)
            {
                var options = new SignalHubSettings();
                _signalHubOptions.Configure(options);

                var data = new JArray();
                result.Steps.Add(new JObject(
                    new JProperty("name", nameof(SignalHubSettings)),
                    new JProperty("settings", JObject.FromObject(options))
                ));
            }

            return Task.CompletedTask;
        }
    }
}
