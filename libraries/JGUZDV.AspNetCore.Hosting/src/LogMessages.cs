﻿using Microsoft.Extensions.Logging;

namespace JGUZDV.AspNetCore.Hosting;

internal partial class LogMessages
{
    [LoggerMessage("Could not find config {configSection}. The corresponding feature will not be added to services or pipeline.")]
    public static partial void MissingConfig(ILogger logger, LogLevel loglevel, string configSection);



    [LoggerMessage(LogLevel.Information, "Could not find config Authentication:JwtBearer:ValidAudiences, audiences will not be considered.")]
    public static partial void NoValidAudiences(ILogger logger);

    [LoggerMessage(LogLevel.Warning, "Could not find config Authentication:JwtBearer:RequiredScopes or Authentication:JwtBearer:AllowedScopes, consider addings required scopes to validate the token is meant for us.")]
    public static partial void NoRequiredScopes(ILogger logger);
}