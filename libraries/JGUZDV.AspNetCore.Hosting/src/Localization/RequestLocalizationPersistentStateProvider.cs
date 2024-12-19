using JGUZDV.AspNetCore.Hosting.Components;

using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.Options;

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
        applicationState.PersistAsJson(nameof(RequestLocalizationState), RequestLocalizationState.FromOptions(_options.Value));
        return Task.CompletedTask;
    }
}
