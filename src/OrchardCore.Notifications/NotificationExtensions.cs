using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;

namespace OrchardCore.Notifications
{
    internal static class NotificationExtensions
    {
        public static IDictionary<string, string> GetProperties(this Notification notification)
        { 
            return new Dictionary<string, string>(notification.Select(p =>
                new KeyValuePair<string, string>(p.Key, ConvertToString(p.Value))));
        }

        private static string ConvertToString(object value) =>
            (value != null) ? TypeDescriptor.GetConverter(value).ConvertToInvariantString(value) : null;
    }
}
