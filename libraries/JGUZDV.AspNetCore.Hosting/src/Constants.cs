﻿namespace JGUZDV.AspNetCore.Hosting;

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

    }
}