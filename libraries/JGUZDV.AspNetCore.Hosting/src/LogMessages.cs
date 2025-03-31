using Microsoft.Extensions.Logging;

namespace JGUZDV.AspNetCore.Hosting;

internal partial class LogMessages
{
    // Generic messages

    [LoggerMessage(LogLevel.Debug, "Added '{feature}' without specific configuration.")]
    public static partial void FeatureAdded(ILogger logger, string feature);

    [LoggerMessage(LogLevel.Debug, "Added '{feature}' with configuration from '{configSection}'")]
    public static partial void FeatureConfigured(ILogger logger, string feature, string configSection);

    [LoggerMessage(LogLevel.Information, "Could not find config '{configSection}'. Optional feature '{feature}' will not be added to services or pipeline.")]
    public static partial void MissingOptionalConfig(ILogger logger, string feature, string configSection);



    // Specific messages

    [LoggerMessage("Configuration file {configFile} has been added as first json file to be loaded.", Level = LogLevel.Information)]
    public static partial void MachineConfigurationFileAdded(ILogger logger, string configFile);


    [LoggerMessage(LogLevel.Information, "Could not find config section '{configSection}' for RequestLocalization. Using default languages: [de-DE, en-US].")]
    public static partial void DefaultRequestLocalizationConfig(ILogger logger, string configSection);


    [LoggerMessage(LogLevel.Information, "Could not find config Authentication:JwtBearer:ValidAudiences, audiences will not be considered.")]
    public static partial void NoValidAudiences(ILogger logger);

    [LoggerMessage(LogLevel.Warning, "Could not find config Authentication:JwtBearer:RequiredScopes or Authentication:JwtBearer:AllowedScopes, consider addings required scopes to validate the token is meant for us.")]
    public static partial void NoRequiredScopes(ILogger logger);


    [LoggerMessage(LogLevel.Warning, "Could not find configuration for DataProtection in config-section '{configSection}'. Generally we configure data-protection machine wide, so you most likely can ignore this message when running in development.")]
    public static partial void DataProtectionMissing(ILogger logger, string configSection);

    [LoggerMessage(LogLevel.Information, "Could not read DataProtection application discriminator from '{configKey}'. In Production a fallback to {defaultDiscriminator} will happen.")]
    public static partial void ApplicationDiscriminatorNotSet(ILogger logger, string configKey);


    [LoggerMessage(LogLevel.Warning, "Could not find configuration for DistributedCache in config-section '{configSection}'. If you do not use IDistributedCache, you can ignore this warning. Else you need to configure and setup the caches accordingly.")]
    public static partial void DistributedCacheMissing(ILogger logger, string configSection);


    [LoggerMessage(LogLevel.Warning, "Coult not find configuration for ForwarededHeaders in config-section '{configSection}'. Generally this is configured machine wide on production machines, so you most likeley can ignore this message.")]
    public static partial void ForwardedHeadersMissing(ILogger logger, string configSection);
}
