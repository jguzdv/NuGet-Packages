using Microsoft.Extensions.Logging;

namespace JGUZDV.AspNetCore.Hosting;

internal partial class LogMessages
{
    [LoggerMessage("Configuration file {configFile} has been added as first json file to be loaded.", Level = LogLevel.Information)]
    public static partial void MachineConfigurationFileAdded(ILogger logger, string configFile);

    [LoggerMessage("Could not find config {configSection}. The corresponding feature will not be added to services or pipeline.")]
    public static partial void MissingConfig(ILogger logger, LogLevel loglevel, string configSection);



    [LoggerMessage(LogLevel.Information, "Could not find config Authentication:JwtBearer:ValidAudiences, audiences will not be considered.")]
    public static partial void NoValidAudiences(ILogger logger);

    [LoggerMessage(LogLevel.Warning, "Could not find config Authentication:JwtBearer:RequiredScopes or Authentication:JwtBearer:AllowedScopes, consider addings required scopes to validate the token is meant for us.")]
    public static partial void NoRequiredScopes(ILogger logger);

    [LoggerMessage(LogLevel.Critical, "Could not read DataProtection application discriminator from {configKey}. This is neccessary for production deploys.")]
    public static partial void ApplicationDiscriminatorNotSet(ILogger logger, string configKey);
}
