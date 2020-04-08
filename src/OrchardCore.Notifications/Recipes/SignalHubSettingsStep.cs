using System;
using System.Threading.Tasks;
using OrchardCore.Notifications.Configuration;
using OrchardCore.Recipes.Models;
using OrchardCore.Recipes.Services;

namespace OrchardCore.Notifications.Recipes
{
    /// <summary>
    /// This recipe step updates the site settings.
    /// </summary>
    internal class SignalHubSettingsStep : IRecipeStepHandler
    {
        private readonly TenantConfigurationStore _tenantConfigStore;

        public SignalHubSettingsStep(TenantConfigurationStore tenantConfigStore)
        {
            _tenantConfigStore = tenantConfigStore;
        }

        public async Task ExecuteAsync(RecipeExecutionContext context)
        {
            if (!String.Equals(context.Name, nameof(SignalHubSettings), StringComparison.OrdinalIgnoreCase))
            {
                return;
            }

            var model = context.Step;
            var settings = model.ToObject<SignalHubSettingsStepModel>().Settings;

            await _tenantConfigStore.SaveAsync(Constants.Features.SignalHub, settings);
        }
    }

    internal class SignalHubSettingsStepModel
    {
        public SignalHubSettings Settings { get; set; }
    }
}
