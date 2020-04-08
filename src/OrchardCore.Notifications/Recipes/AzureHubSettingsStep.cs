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
    internal class AzureHubSettingsStep : IRecipeStepHandler
    {
        private readonly TenantConfigurationStore _tenantConfigStore;

        public AzureHubSettingsStep(TenantConfigurationStore tenantConfigStore)
        {
            _tenantConfigStore = tenantConfigStore;
        }

        public async Task ExecuteAsync(RecipeExecutionContext context)
        {
            if (!String.Equals(context.Name, nameof(AzureHubSettings), StringComparison.OrdinalIgnoreCase))
            {
                return;
            }

            var model = context.Step;
            var settings = model.ToObject<AzureHubSettingsStepModel>().Settings;

            await _tenantConfigStore.SaveAsync(Constants.Features.AzureHub, settings);
        }
    }

    internal class AzureHubSettingsStepModel
    {
        public AzureHubSettings Settings { get; set; }
    }
}
