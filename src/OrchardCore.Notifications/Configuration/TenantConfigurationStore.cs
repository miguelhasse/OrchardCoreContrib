using System.IO;
using System.Threading.Tasks;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using OrchardCore.Environment.Shell;

namespace OrchardCore.Notifications.Configuration
{
    internal sealed class TenantConfigurationStore
    {
        private readonly string _container;
        private readonly string _tenant;

        public TenantConfigurationStore(ShellSettings currentShellSettings, IOptions<ShellOptions> shellOptions)
        {
            _tenant = currentShellSettings.Name;
            // e.g., App_Data/Sites
            _container = Path.Combine(shellOptions.Value.ShellsApplicationDataPath, shellOptions.Value.ShellsContainerName);
            Directory.CreateDirectory(_container);
        }

        public async Task SaveAsync(string path, object settings)
        {
            var tenantFolder = Path.Combine(_container, _tenant);
            var appsettings = Path.Combine(tenantFolder, "appsettings.json");

            JObject config;
            if (File.Exists(appsettings))
            {
                using (var file = File.OpenText(appsettings))
                {
                    using var reader = new JsonTextReader(file);
                    config = await JObject.LoadAsync(reader);
                }
            }
            else
            {
                config = new JObject();
            }

            config[path] = JObject.FromObject(settings);
            Directory.CreateDirectory(tenantFolder);

            using (var file = File.CreateText(appsettings))
            {
                using var writer = new JsonTextWriter(file) { Formatting = Formatting.Indented };
                await config.WriteToAsync(writer);
            }
        }
    }
}
