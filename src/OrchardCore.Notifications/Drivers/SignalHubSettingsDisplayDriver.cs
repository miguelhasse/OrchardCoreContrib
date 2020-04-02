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
    public class SignalHubSettingsDisplayDriver : SectionDisplayDriver<ISite, SignalHubSettings>
    {
        public const string GroupId = Constants.Features.SignalHub;

        private readonly IDataProtectionProvider _dataProtectionProvider;
        private readonly IShellHost _orchardHost;
        private readonly ShellSettings _currentShellSettings;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IAuthorizationService _authorizationService;

        public SignalHubSettingsDisplayDriver(
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

            var section = new SignalHubSettings();
            configuration.GetSection(GroupId).Bind(section);

            var shapes = new List<IDisplayResult>
            {
                Initialize<SignalHubSettingsViewModel>("SignalHubSettings_Edit", model =>
                {
                    model.Connection = section.Connection;
                    model.UseLocal = section.UseLocal;
                    model.UseMessagePack = section.UseMessagePack;
                    model.RedisBackplane = section.RedisBackplane;
                })
                .Location("Content")
                .OnGroup(GroupId)
            };

            if (section.UseLocal || section.Connection != null)
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
                var viewmodel = new SignalHubSettingsViewModel();
                await context.Updater.TryUpdateModelAsync(viewmodel, Prefix);

                configuration[string.Join(':', GroupId, nameof(SignalHubSettings.Connection))] = viewmodel.Connection;
                configuration[string.Join(':', GroupId, nameof(SignalHubSettings.UseLocal))] = viewmodel.UseLocal.ToString();
                configuration[string.Join(':', GroupId, nameof(SignalHubSettings.UseMessagePack))] = viewmodel.UseMessagePack.ToString();
                configuration[string.Join(':', GroupId, nameof(SignalHubSettings.RedisBackplane))] = viewmodel.RedisBackplane;

                // Reload the tenant to apply the settings
                await _orchardHost.UpdateShellSettingsAsync(_currentShellSettings);
            }

            return await EditAsync(model, context);
        }
    }
}
