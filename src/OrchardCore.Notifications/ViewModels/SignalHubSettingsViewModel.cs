using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq.Expressions;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Localization;

namespace OrchardCore.Notifications.ViewModels
{
    /// <summary>
    /// Represents a settings for SMTP.
    /// </summary>
    public class SignalHubSettingsViewModel : IValidatableObject
    {
        private static readonly Lazy<Func<string, (string endpoint, string accessKey, string version, int? port)>> _connectionStringParser =
            new Lazy<Func<string, (string endpoint, string accessKey, string version, int? port)>>(() => GetConnectionStringParserFunc());

        public string Connection { get; set; }

        public bool UseLocal { get; set; }

        public bool UseMessagePack { get; set; }

        public string RedisBackplane { get; set; }

        public string Endpoint { get; private set; }

        public string SharedAccessKey { get; private set; }

        /// <inheritdocs />
        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            var S = validationContext.GetService<IStringLocalizer<AzureHubSettingsViewModel>>();
            Endpoint = SharedAccessKey = null;

            if (UseLocal)
            {
                if (string.IsNullOrWhiteSpace(RedisBackplane) == false
                    && TryParseRedisConfiguration(RedisBackplane, out StackExchange.Redis.ConfigurationOptions options, out Exception exception) == false)
                {
                    yield return new ValidationResult(exception.Message, new[] { nameof(RedisBackplane) });
                }
            }
            else
            {
                if (TryParseConnectionString(Connection, out string endpoint, out string accessKey, out Exception exception))
                {
                    Endpoint = endpoint;
                    SharedAccessKey = accessKey;
                }
                else
                {
                    yield return new ValidationResult(exception.Message, new[] { nameof(Connection) });
                }
            }

            yield break;
        }

        private static bool TryParseConnectionString(string connectionString, out string endpoint, out string accessKey, out Exception exception)
        {
            try
            {
                var result = _connectionStringParser.Value.Invoke(connectionString);
                endpoint = result.endpoint;
                accessKey = result.accessKey;
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

        private static bool TryParseRedisConfiguration(string configuration, out StackExchange.Redis.ConfigurationOptions options, out Exception exception)
        {
            try
            {
                options = StackExchange.Redis.ConfigurationOptions.Parse(configuration);
                exception = null;
                return true;
            }
            catch (Exception ex)
            {
                options = null;
                exception = ex;
                return false;
            }
        }

        private static Func<string, (string endpoint, string accessKey, string version, int? port)> GetConnectionStringParserFunc()
        {
            var type = typeof(Microsoft.Azure.SignalR.ServiceEndpoint).Assembly.GetType("Microsoft.Azure.SignalR.ConnectionStringParser");
            var methodInfo = type.GetMethod("Parse", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Static);

            var source = Expression.Parameter(typeof(string));
            return Expression.Lambda<Func<string, (string endpoint, string accessKey, string version, int? port)>>(
                Expression.Call(methodInfo, source), source).Compile();
        }
    }
}
