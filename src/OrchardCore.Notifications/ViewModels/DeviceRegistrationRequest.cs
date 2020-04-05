using System.ComponentModel.DataAnnotations;

namespace OrchardCore.Notifications.ViewModels
{
    public class DeviceRegistrationRequest
    {
        public string Handle { get; set; }

        [RegularExpression(@"([-A-Za-z0-9_]+", ErrorMessage = "Invalid tag name")]
        public string Tag { get; set; }
    }
}
