using OrchardCore.Deployment;

namespace OrchardCore.Notifications.Deployment
{
    public class SettingsDeploymentStep : DeploymentStep
    {
        public SettingsDeploymentStep()
        {
            Name = "NotificationSettings";
        }

        public bool IncludeAzureHubSettings { get; set; } = true;

        public bool IncludeSignalHubSettings { get; set; } = true;
    }
}
