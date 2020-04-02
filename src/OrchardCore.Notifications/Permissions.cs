using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using OrchardCore.Security.Permissions;

namespace OrchardCore.Notifications
{
    public class Permissions : IPermissionProvider
    {
        public static readonly Permission ManageNotificationSettings = new Permission("ManageNotificationSettings", "Manage Notification Settings");

        public Task<IEnumerable<Permission>> GetPermissionsAsync()
        {
            return Task.FromResult(new[]
            {
                ManageNotificationSettings
            }
            .AsEnumerable());
        }

        public IEnumerable<PermissionStereotype> GetDefaultStereotypes()
        {
            return new[]
            {
                new PermissionStereotype
                {
                    Name = "Administrator",
                    Permissions = new[] { ManageNotificationSettings }
                },
            };
        }
    }
}
