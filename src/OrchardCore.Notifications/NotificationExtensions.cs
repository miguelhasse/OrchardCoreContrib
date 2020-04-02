using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json.Linq;

namespace OrchardCore.Notifications
{
    internal static class NotificationExtensions
    {
        public static IDictionary<string, string> GetProperties(this Notification notification)
        { 
            return new Dictionary<string, string>(notification.Select(p =>
                new KeyValuePair<string, string>(p.Key, p.Value != null ? JToken.FromObject(p.Value).ToString() : null)));
        }
    }
}
