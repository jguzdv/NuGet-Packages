namespace JGUZDV.AspNetCore.Hosting;

/// <summary>
/// Collection of constants for Hosting.
/// </summary>
public static class Constants
{
    /// <summary>
    /// Collection of constants for configuration sections.
    /// </summary>
    public static class ConfigSections
    {
        /// <summary>
        /// Default Configuration section for Authentication.
        /// </summary>
        public const string Authentication = "Authentication";

        /// <summary>
        /// Default Configuration section for DataProtection.
        /// </summary>
        public const string DataProtection = "JGUZDV:DataProtection";

        /// <summary>
        /// Default Configuration section for DistributedCache.
        /// </summary>
        public const string DistributedCache = "DistributedCache";

        /// <summary>
        /// Default Configuration section for Telemetry.
        /// </summary>
        public const string Telemetry = "ApplicationInsights";
    }
}