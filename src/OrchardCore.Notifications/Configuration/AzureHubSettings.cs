namespace OrchardCore.Notifications.Configuration
{
    /// <summary>
    /// Represents a settings for SMTP.
    /// </summary>
    public class AzureHubSettings
    {
        /// <summary>
        /// Gets or sets the hub name.
        /// </summary>
        public string Hub { get; set; }

        /// <summary>
        /// Gets or sets the connection string to the named hub.
        /// </summary>
        public string Connection { get; set; }
    }
}
