using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json.Linq;
using OrchardCore.DisplayManagement.Entities;
using OrchardCore.DisplayManagement.Handlers;
using OrchardCore.DisplayManagement.Views;
using OrchardCore.Environment.Shell;
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

            var notificationTags = (model.Properties.TryGetValue(nameof(AzureHubSettingsViewModel.NotificationTags), out var tagsprop)
                && tagsprop.Type == JTokenType.Array) ? string.Join(", ", tagsprop.ToArray().Select(s => s.ToString())) : null;

            var shapes = new List<IDisplayResult>
            {
                Initialize<AzureHubSettingsViewModel>("AzureHubSettings_Edit", model =>
                {
                    model.Hub = section.Hub;
                    model.Connection = section.Connection;
                    model.NotificationTags = notificationTags;
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

                if (!string.IsNullOrWhiteSpace(viewmodel.NotificationTags))
                {
                    model.Properties.TryAdd(nameof(AzureHubSettingsViewModel.NotificationTags),
                        JArray.FromObject(viewmodel.NotificationTags.Split(',').Select(t => t.Trim())));
                }
                else model.Properties.Remove(nameof(AzureHubSettingsViewModel.NotificationTags));

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
