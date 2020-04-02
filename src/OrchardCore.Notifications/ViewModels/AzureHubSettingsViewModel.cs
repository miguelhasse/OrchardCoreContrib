using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Microsoft.Azure.NotificationHubs;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Localization;

namespace OrchardCore.Notifications.ViewModels
{
    /// <summary>
    /// Represents a settings for SMTP.
    /// </summary>
    public class AzureHubSettingsViewModel : IValidatableObject
    {
        [MinLength(1, ErrorMessage = "Hub name length to short")]
		[MaxLength(256, ErrorMessage = "Hub name length to long")]
        [RegularExpression(@"[A-Za-z0-9][-A-Za-z0-9_.]+[-A-Za-z0-9_]", ErrorMessage = "Invalid hub name")]
        public string Hub { get; set; }

        [Required(AllowEmptyStrings = false)]
        public string Connection { get; set; }

        public string Endpoint { get; private set; }

        public string SharedAccessKey { get; private set; }

        /// <inheritdocs />
        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            var S = validationContext.GetService<IStringLocalizer<AzureHubSettingsViewModel>>();
            Endpoint = SharedAccessKey = null;

            if (TryParseConnectionString(Connection, out string endpoint, out string accessKey, out Exception exception))
            {
                Endpoint = endpoint;
                SharedAccessKey = accessKey;
            }
            else
            {
                yield return new ValidationResult(exception.Message, new[] { nameof(Connection) });
            }

            yield break;
        }

        private static bool TryParseConnectionString(string connectionString, out string endpoint, out string accessKey, out Exception exception)
        {
            try
            {
                var result = new NotificationHubConnectionStringBuilder(connectionString);
                endpoint = result.Endpoint.AbsoluteUri;
                accessKey = result.SharedAccessKey;
                exception = null;
                return true;
            }
            catch (Exception ex)
            {
                endpoint = accessKey = null;
                exception = ex;
                return false;
            }
        }
    }
}
