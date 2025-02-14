﻿using System.Globalization;

using JGUZDV.AspNetCore.Hosting.Components;

using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.Options;

using static IdentityModel.ClaimComparer;

namespace JGUZDV.AspNetCore.Hosting.Localization;

/// <summary>
/// This middleware will serialize the current request's localization settings to PersistentComponentState.
/// </summary>
public class RequestLocalizationPersistentStateProvider : IPersistentComponentStateProvider
{
    private readonly IOptions<RequestLocalizationOptions> _options;

    /// <summary>
    /// Creates a new instance of the RequestLocalizationSerializationMiddleware.
    /// </summary>
    public RequestLocalizationPersistentStateProvider(IOptions<RequestLocalizationOptions> options)
    {
        _options = options;
    }

    /// <inheritdoc />
    public Task PersistStateAsync(PersistentComponentState applicationState)
    {
        var requestLocalizationState = new RequestLocalizationState
        {
            CurrentCulture = CultureInfo.CurrentCulture.ToString(),
            CurrentUICulture = CultureInfo.CurrentUICulture.ToString(),

            SupportedCultures = [.. _options.Value.SupportedCultures?.Select(c => new LocalizationInfo(c.ToString(), c.NativeName))],
            SupportedUICultures = [.. _options.Value.SupportedUICultures?.Select(c => new LocalizationInfo(c.ToString(), c.NativeName))]
        };

        applicationState.PersistAsJson(nameof(RequestLocalizationState), requestLocalizationState);
        return Task.CompletedTask;
    }
}
