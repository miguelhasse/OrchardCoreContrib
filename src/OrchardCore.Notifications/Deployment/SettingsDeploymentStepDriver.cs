using OrchardCore.Deployment;
using OrchardCore.DisplayManagement.Handlers;
using OrchardCore.DisplayManagement.Views;

namespace OrchardCore.Notifications.Deployment
{
    public class SettingsDeploymentStepDriver : DisplayDriver<DeploymentStep, SettingsDeploymentStep>
    {
        public override IDisplayResult Display(SettingsDeploymentStep step)
        {
            return Combine(
                View("SettingsDeploymentStep_Fields_Summary", step).Location("Summary", "Content"),
                View("SettingsDeploymentStep_Fields_Thumbnail", step).Location("Thumbnail", "Content")
            );
        }

        public override IDisplayResult Edit(SettingsDeploymentStep step)
        {
            return View("SettingsDeploymentStep_Fields_Edit", step).Location("Content");
        }
    }
}
