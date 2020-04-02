using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using OrchardCore.DisplayManagement.Entities;
using OrchardCore.DisplayManagement.Handlers;
using OrchardCore.DisplayManagement.Views;
using OrchardCore.Environment.Shell;
using OrchardCore.Environment.Shell.Configuration;
using OrchardCore.Notifications.Configuration;
using OrchardCore.Notifications.ViewModels;
using OrchardCore.Settings;

namespace OrchardCore.Notifications.Drivers
{
    internal class AzureHubSettingsDisplayDriver : SectionDisplayDriver<ISite, AzureHubSettings>
    {
        public const string GroupId = Constants.Features.AzureHub;

        private readonly IDataProtectionProvider _dataProtectionProvider;
        private readonly IShellHost _orchardHost;
        private readonly ShellSettings _currentShellSettings;
        private readonly TenantConfigurationStore _tenantConfigStore;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IAuthorizationService _authorizationService;

        public AzureHubSettingsDisplayDriver(
            IDataProtectionProvider dataProtectionProvider,
            IShellHost orchardHost,
            ShellSettings currentShellSettings,
            TenantConfigurationStore tenantConfigStore,
            IHttpContextAccessor httpContextAccessor,
            IAuthorizationService authorizationService)
        {
            _dataProtectionProvider = dataProtectionProvider;
            _orchardHost = orchardHost;
            _currentShellSettings = currentShellSettings;
            _tenantConfigStore = tenantConfigStore;
            _httpContextAccessor = httpContextAccessor;
            _authorizationService = authorizationService;
        }

        public override async Task<IDisplayResult> EditAsync(ISite model, BuildEditorContext context)
        {
            var user = _httpContextAccessor.HttpContext?.User;
            var configuration = _currentShellSettings.ShellConfiguration;

            if (!await _authorizationService.AuthorizeAsync(user, Permissions.ManageNotificationSettings))
            {
                return null;
            }

            var section = new AzureHubSettings();
            configuration.GetSection(GroupId).Bind(section);

            var shapes = new List<IDisplayResult>
            {
                Initialize<AzureHubSettingsViewModel>("AzureHubSettings_Edit", model =>
                {
                    model.Hub = section.Hub;
                    model.Connection = section.Connection;
                })
                .Location("Content")
                .OnGroup(GroupId)
            };

            if (section.Hub != null && section.Connection != null)
            {
                shapes.Add(Dynamic("Notification_TestButton")
                    .Location("Actions")
                    .OnGroup(GroupId));
            }

            return Combine(shapes);
        }

        public override async Task<IDisplayResult> UpdateAsync(ISite model, UpdateEditorContext context)
        {
            var user = _httpContextAccessor.HttpContext?.User;

            if (!await _authorizationService.AuthorizeAsync(user, Permissions.ManageNotificationSettings))
            {
                return null;
            }

            if (context.GroupId == GroupId)
            {
                var viewmodel = new AzureHubSettingsViewModel();
                await context.Updater.TryUpdateModelAsync(viewmodel, Prefix);

                var settings = new AzureHubSettings
                {
                    Hub = viewmodel.Hub,
                    Connection = viewmodel.Connection
                };
                
                await _tenantConfigStore.SaveAsync(GroupId, settings);

                // Reload the tenant to apply the settings
                await _orchardHost.ReloadShellContextAsync(_currentShellSettings);
            }

            return await EditAsync(model, context);
        }
    }
}
