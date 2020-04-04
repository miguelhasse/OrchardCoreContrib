using System;
using System.Linq;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Azure.NotificationHubs;
using Microsoft.Azure.NotificationHubs.Messaging;
using Microsoft.Extensions.Logging;
using OrchardCore.Settings;
using Newtonsoft.Json.Linq;
using OrchardCore.Notifications.ViewModels;

namespace OrchardCore.Notifications.Hubs
{
    internal class AzureHubService : IPushNotificationService
    {
        private readonly ISiteService _siteService;
        private readonly INotificationHubFactory _factory;
        private readonly ILogger<AzureHubService> _logger;

        public AzureHubService(ISiteService siteService, INotificationHubFactory factory, ILogger<AzureHubService> logger)
        {
            _siteService = siteService;
            _factory = factory;
            _logger = logger;
        }

        public async Task<string> CreateRegistrationId(string handle)
        {
            var hub = _factory.NotificationHubClient;
            string newRegistrationId = null;

            if (handle != null)
            {
                var registrations = await hub.GetRegistrationsByChannelAsync(handle, 100);

                foreach (RegistrationDescription registration in registrations)
                {
                    if (newRegistrationId == null)
                    {
                        newRegistrationId = registration.RegistrationId;
                        continue;
                    }

                    await hub.DeleteRegistrationAsync(registration);
                }
            }

            return newRegistrationId ?? await hub.CreateRegistrationIdAsync();
        }

        public async Task Register(MobilePlatform platform, string handle, string registrationId, IEnumerable<string> tags)
        {
            var hub = _factory.NotificationHubClient;
            RegistrationDescription registrationDescription = null;

            switch (platform)
            {
                case MobilePlatform.Windows:
                    registrationDescription = new WindowsRegistrationDescription(handle);
                    break;
                case MobilePlatform.Apple:
                    registrationDescription = new AppleRegistrationDescription(handle);
                    break;
                case MobilePlatform.Android:
                    registrationDescription = new FcmRegistrationDescription(handle);
                    break;
            }

            registrationDescription.RegistrationId = registrationId;

            if (tags != null)
            {
                registrationDescription.Tags = new HashSet<string>(tags);
            }

            try
            {
                await hub.CreateOrUpdateRegistrationAsync(registrationDescription);
            }
            catch (MessagingException exception)
            {
                _logger.LogError(exception, "Unhandled exception was thrown during registration in Azure Notification Hub");
            }
        }

        public async Task Unregister(string registrationId)
        {
            await _factory.NotificationHubClient.DeleteRegistrationAsync(registrationId);
        }

        public async Task<string> SendNotification(IDictionary<string, string> properties)
        {
            var hub = _factory.NotificationHubClient;
            var tags = await GetNotificationTags();

            if (properties.Remove(nameof(Notification.Tags), out string tagexp) && tagexp != null)
            {
                var tagprops = tagexp.Split(',').Select(t => t.Trim());
                tags = ((tags != null) ? tags.Union(tagprops) : tagprops).Distinct().Take(20);
            }

            try
            {
                var outcome = (tags != null)
                    ? await hub.SendTemplateNotificationAsync(properties, tags)
                    : await hub.SendTemplateNotificationAsync(properties);
                return (outcome != null && outcome.State <= NotificationOutcomeState.Completed) ? outcome.TrackingId : null;
            }
            catch (MessagingException exception)
            {
                _logger.LogError(exception, "Unhandled exception was thrown during send notification to Azure Notification Hub");
                throw;
            }
        }

        private async Task<IEnumerable<string>> GetNotificationTags()
        {
            var jtags = (await _siteService.GetSiteSettingsAsync()).Properties[nameof(AzureHubSettingsViewModel.NotificationTags)];
            return (jtags != null && jtags.Type == JTokenType.Array) ? jtags.Values<string>() : null;
        }
    }
}
