using Microsoft.Extensions.Logging;

namespace JGUZDV.AspNetCore.Hosting;

internal partial class LogMessages
{
    [LoggerMessage(LogLevel.Debug, "Added {feature} without specific configuration.")]
    public static partial void FeatureAdded(ILogger logger, string feature);

    [LoggerMessage(LogLevel.Debug, "Added {feature} with configuration from {configSection}")]
    public static partial void FeatureConfigured(ILogger logger, string feature, string configSection);

    [LoggerMessage("Could not find config {configSection}. {feature} will not be added to services or pipeline.")]
    public static partial void MissingConfig(ILogger logger, LogLevel loglevel, string feature, string configSection);


    [LoggerMessage("Configuration file {configFile} has been added as first json file to be loaded.", Level = LogLevel.Information)]
    public static partial void MachineConfigurationFileAdded(ILogger logger, string configFile);




    [LoggerMessage(LogLevel.Information, "Could not find config Authentication:JwtBearer:ValidAudiences, audiences will not be considered.")]
    public static partial void NoValidAudiences(ILogger logger);

    [LoggerMessage(LogLevel.Warning, "Could not find config Authentication:JwtBearer:RequiredScopes or Authentication:JwtBearer:AllowedScopes, consider addings required scopes to validate the token is meant for us.")]
    public static partial void NoRequiredScopes(ILogger logger);


    [LoggerMessage(LogLevel.Critical, "Could not read DataProtection application discriminator from {configKey}. This is neccessary for production deploys.")]
    public static partial void ApplicationDiscriminatorNotSet(ILogger logger, string configKey);

    [LoggerMessage(LogLevel.Critical, "A missing distributed cache config will lead to problems, when the software is running in a farm environment.")]
    public static partial void DistributedCacheMissing(ILogger logger);
}
