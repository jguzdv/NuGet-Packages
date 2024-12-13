namespace JGUZDV.AspNetCore.Hosting
{
    /// <summary>
    /// Exception thrown when a configuration section is missing.
    /// </summary>
    public class ConfigurationMissingException : InvalidOperationException
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ConfigurationMissingException"/> class.
        /// </summary>
        /// <param name="configSection">The missing configuration section.</param>
        public ConfigurationMissingException(string configSection)
            : base($"Configuration section '{configSection}' is missing.")
        {
        }
    }
}
