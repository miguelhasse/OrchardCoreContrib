using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using OrchardCore.DisplayManagement.Entities;
using OrchardCore.DisplayManagement.Handlers;
using OrchardCore.DisplayManagement.Views;
using OrchardCore.Environment.Shell;
using OrchardCore.Notifications.Configuration;
using OrchardCore.Notifications.ViewModels;
using OrchardCore.Settings;

namespace OrchardCore.Notifications.Drivers
{
    public class AzureHubSettingsDisplayDriver : SectionDisplayDriver<ISite, AzureHubSettings>
    {
        public const string GroupId = Constants.Features.AzureHub;

        private readonly IDataProtectionProvider _dataProtectionProvider;
        private readonly IShellHost _orchardHost;
        private readonly ShellSettings _currentShellSettings;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IAuthorizationService _authorizationService;

        public AzureHubSettingsDisplayDriver(
            IDataProtectionProvider dataProtectionProvider,
            IShellHost orchardHost,
            ShellSettings currentShellSettings,
            IHttpContextAccessor httpContextAccessor,
            IAuthorizationService authorizationService)
        {
            _dataProtectionProvider = dataProtectionProvider;
            _orchardHost = orchardHost;
            _currentShellSettings = currentShellSettings;
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
            var configuration = _currentShellSettings.ShellConfiguration;

            if (!await _authorizationService.AuthorizeAsync(user, Permissions.ManageNotificationSettings))
            {
                return null;
            }

            if (context.GroupId == GroupId)
            {
                var viewmodel = new AzureHubSettingsViewModel();
                await context.Updater.TryUpdateModelAsync(viewmodel, Prefix);

                var section = configuration.GetSection(GroupId);

                section[nameof(AzureHubSettings.Hub)] = viewmodel.Hub;
                section[nameof(AzureHubSettings.Connection)] = viewmodel.Connection;

                // Reload the tenant to apply the settings
                await _orchardHost.ReloadShellContextAsync(_currentShellSettings);
            }

            return await EditAsync(model, context);
        }
    }
}
