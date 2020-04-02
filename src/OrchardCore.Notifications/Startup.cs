using System;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Routing;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Options;
using OrchardCore.Admin;
using OrchardCore.DisplayManagement.Handlers;
using OrchardCore.Environment.Shell;
using OrchardCore.Modules;
using OrchardCore.Mvc.Core.Utilities;
using OrchardCore.Navigation;
using OrchardCore.Notifications.Configuration;
using OrchardCore.Notifications.Controllers;
using OrchardCore.Notifications.Drivers;
using OrchardCore.Notifications.Hubs;
using OrchardCore.Security.Permissions;
using OrchardCore.Settings;

namespace OrchardCore.Notifications
{
    public class Startup : StartupBase
    {
        private readonly AdminOptions _adminOptions;

        public Startup(IOptions<AdminOptions> adminOptions)
        {
            _adminOptions = adminOptions.Value;
        }

        public override void ConfigureServices(IServiceCollection services)
        {
            // Note: the following services are registered using TryAddEnumerable to prevent duplicate registrations.
            services.TryAddEnumerable(new[]
            {
                ServiceDescriptor.Scoped<IPermissionProvider, Permissions>(),
                ServiceDescriptor.Scoped<INavigationProvider, AdminMenu>(),
            });

            services.AddScoped<IEventPublisher, DefaultEventPublisher>();
            services.AddScoped<TenantConfigurationStore>();
        }

        public override void Configure(IApplicationBuilder app, IEndpointRouteBuilder routes, IServiceProvider serviceProvider)
        {
            routes.MapAreaControllerRoute(
                name: "NotificationIndex",
                areaName: Constants.Features.Core,
                pattern: $"{_adminOptions.AdminUrlPrefix}/Notification/Index",
                defaults: new { controller = typeof(AdminController).ControllerName(), action = nameof(AdminController.Index) }
            );
        }
    }

    [Feature(Constants.Features.SignalHub)]
    [RequireFeatures(Constants.Features.Core)]
    public class SignalHubStartup : StartupBase
    {
        private readonly ShellSettings _shellSettings;

        public SignalHubStartup(ShellSettings shellSettings)
        {
            _shellSettings = shellSettings;
        }

        public override void ConfigureServices(IServiceCollection services)
        {
            services.TryAddEnumerable(new[]
            {
                ServiceDescriptor.Scoped<IDisplayDriver<ISite>, SignalHubSettingsDisplayDriver>(),
                ServiceDescriptor.Scoped<IConfigureOptions<SignalHubSettings>, SignalHubSettingsConfiguration>(),
            });

            services.AddScoped<IEventHandler<Notification>, SignalHubHandler>();

            var settings = GetSettings();
            var signalRBuilder = services.AddSignalR();

            if (settings.UseLocal)
            {
                if (settings.UseMessagePack)
                    signalRBuilder.AddMessagePackProtocol();

                if (string.IsNullOrWhiteSpace(settings.RedisBackplane) == false)
                    signalRBuilder.AddStackExchangeRedis(settings.RedisBackplane);
            }
            else
            {
                signalRBuilder.AddAzureSignalR(settings.Connection);
            }
        }

        public override void Configure(IApplicationBuilder builder, IEndpointRouteBuilder routes, IServiceProvider serviceProvider)
        {
            routes.MapHub<LocalHub>("/notification");
        }

        private SignalHubSettings GetSettings()
        {
            var settings = new SignalHubSettings();
            _shellSettings.ShellConfiguration.GetSection(Constants.Features.SignalHub).Bind(settings);
            return settings;
        }

        private class SignalRServerBuilder : ISignalRServerBuilder
        {
            public SignalRServerBuilder(IServiceCollection services) => Services = services;

            public IServiceCollection Services { get; }
        }
    }

    [Feature(Constants.Features.AzureHub)]
    [RequireFeatures(Constants.Features.Core)]
    public class AzureHubStartup : StartupBase
    {
        private readonly ShellSettings _shellSettings;

        public AzureHubStartup(ShellSettings shellSettings)
        {
            _shellSettings = shellSettings;
        }

        public override void ConfigureServices(IServiceCollection services)
        {
            // Note: the following services are registered using TryAddEnumerable to prevent duplicate registrations.
            services.TryAddEnumerable(new[]
            {
                ServiceDescriptor.Scoped<IDisplayDriver<ISite>, AzureHubSettingsDisplayDriver>(),
                ServiceDescriptor.Scoped<IConfigureOptions<AzureHubSettings>, AzureHubSettingsConfiguration>(),
            });

            var settings = GetSettings();

            services.AddScoped<IEventHandler<Notification>, AzureHubHandler>();
            services.AddSingleton<INotificationHubFactory>(sp => new AzureHubFactory(settings));
            services.AddSingleton<IPushNotificationService, AzureHubService>();
        }

        private AzureHubSettings GetSettings()
        {
            var settings = new AzureHubSettings();
            _shellSettings.ShellConfiguration.GetSection(Constants.Features.AzureHub).Bind(settings);
            return settings;
        }
    }
}
