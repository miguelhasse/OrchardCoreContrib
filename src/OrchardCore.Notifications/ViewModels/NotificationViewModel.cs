using System;
using System.Collections.Generic;
using System.Linq;

namespace OrchardCore.Notifications.ViewModels
{
    public class NotificationViewModel
    {
        public IList<NotificationProperty> Properties { get; set; }

        public Notification ToModel()
        {
            var properties = Properties.Where(s => !string.IsNullOrWhiteSpace(s.Name))
                .Select(s => new KeyValuePair<string, object>(s.Name, s.Value));

            return new Notification(new Dictionary<string, object>(properties, StringComparer.OrdinalIgnoreCase));
        }

        public static NotificationViewModel FromModel(Notification notification)
        {
            return new NotificationViewModel
            {
                Properties = new List<NotificationProperty>
                {
                    new NotificationProperty(nameof(notification.Tags), string.Join(",", notification.Tags)),
                    new NotificationProperty(nameof(notification.TimeStamp), notification.TimeStamp.ToString())
                }
            };
        }
    }
}
