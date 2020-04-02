namespace OrchardCore.Notifications.Configuration
{
    /// <summary>
    /// Represents a settings for SMTP.
    /// </summary>
    public class SignalHubSettings
    {
        /// <summary>
        /// Gets or sets the connection string to the named hub.
        /// </summary>
        public string Connection { get; set; }

        public bool UseLocal { get; set; }

        public bool UseMessagePack { get; set; }

        public string RedisBackplane { get; set; }
    }
}
