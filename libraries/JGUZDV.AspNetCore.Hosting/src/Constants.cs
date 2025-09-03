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
        /// Default Configuration section for ForwardedHeaders.
        /// </summary>
        public const string ForwardedHeaders = "ForwardedHeaders";


        /// <summary>
        /// Default Configuration section for DistributedCache.
        /// </summary>
        public const string DistributedCache = "DistributedCache";

        /// <summary>
        /// Default Configuration section for Telemetry.
        /// </summary>
        public const string OpenTelemetry = "OpenTelemetry";

        /// <summary>
        /// Default Configuration section for FeatureManagement.
        /// </summary>
        public const string FeatureManagement = "Features";


        /// <summary>
        /// Default Configuration section for RequestLocalization.
        /// </summary>
        public const string RequestLocalization = "RequestLocalization";

        /// <summary>
        /// Default Configuration section for Cookie Authentication.
        /// </summary>
        public const string CookieAuthentication = "Authentication:Cookie";

        /// <summary>
        /// Default Configuration section for JwtBearer Authentication.
        /// </summary>
        public const string JwtBearerAuthentication = "Authentication:JwtBearer";

        /// <summary>
        /// Default Configuration section for OpenIdConnect Authentication.
        /// </summary>
        public const string OpenIdConnectAuthentication = "Authentication:OpenIdConnect";

        /// <summary>
        /// Default Configuration section for ReverseProxy.
        /// </summary>
        public const string ReverseProxy = "ReverseProxy";


        /// <summary>
        /// Default Configuration section for Logging.
        /// </summary>
        public const string FileLogging = "Logging:File";
    }
}