using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using Newtonsoft.Json;

namespace OrchardCore.Notifications.ViewModels
{
    public class NotificationProperty
    {
        private static readonly IEnumerable<string> _reservedNames = new[] { nameof(Notification.Tags), nameof(Notification.TimeStamp) };

        public NotificationProperty() { }

        public NotificationProperty(string name, string value)
        {
            Name = name;
            Value = value;
        }

        [Required, RegularExpression(@"[A-Za-z0-9]+", ErrorMessage = "Only alphanumeric characters allowed.")]
        public string Name { get; set; }

        public string Value { get; set; }

        [JsonIgnore]
        public bool Reserved => _reservedNames.Contains(Name, StringComparer.OrdinalIgnoreCase);
    }
}
