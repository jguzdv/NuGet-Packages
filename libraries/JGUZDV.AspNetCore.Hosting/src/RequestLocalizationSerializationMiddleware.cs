
using System.Globalization;

using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.Extensions.Options;

namespace JGUZDV.AspNetCore.Hosting;

/// <summary>
/// This middleware will serialize the current request's localization settings to PersistentComponentState.
/// </summary>
public class RequestLocalizationSerializationMiddleware : IDisposable
{
    private readonly RequestDelegate _next;
    private readonly PersistentComponentState _applicationState;
    private readonly IOptions<RequestLocalizationOptions> _options;
    private PersistingComponentStateSubscription? _subscription;

    /// <summary>
    /// Creates a new instance of the RequestLocalizationSerializationMiddleware.
    /// </summary>
    public RequestLocalizationSerializationMiddleware(RequestDelegate next, PersistentComponentState applicationState, IOptions<RequestLocalizationOptions> options)
    {
        _next = next;

        _applicationState = applicationState;
        _options = options;
    }

    /// <summary>
    /// Invokes the middleware. This will register a callback to persist the current request's localization settings.
    /// </summary>
    public Task Invoke(HttpContext context)
    {
        _subscription = _applicationState.RegisterOnPersisting(OnPersistState, RenderMode.InteractiveWebAssembly);
        return _next(context);
    }

    /// <inheritdoc />
    public void Dispose()
    {
        _subscription?.Dispose();
    }

    private Task OnPersistState()
    {
        _applicationState.PersistAsJson(nameof(RequestLocalizationOptions), RequestLocalizationState.FromOptions(_options.Value));
        return Task.CompletedTask;
    }
}

/// <summary>
/// Represents the state of the request localization.
/// </summary>
public class RequestLocalizationState
{
    internal static RequestLocalizationState FromOptions(RequestLocalizationOptions options)
        => new RequestLocalizationState
        {
            CurrentCulture = CultureInfo.CurrentCulture.ToString(),
            CurrentUICulture = CultureInfo.CurrentUICulture.ToString(),

            SupportedCultures = [..options.SupportedCultures?.Select(c => new LocalizationInfo(c.ToString(), c.NativeName))],
            SupportedUICultures = [.. options.SupportedUICultures?.Select(c => new LocalizationInfo(c.ToString(), c.NativeName))]
        };


    /// <summary>
    /// The current culture of the request, e.g. de-DE.
    /// </summary>
    public required string CurrentCulture { get; init; }

    /// <summary>
    /// The current UI culture of the request, e.g. de-DE
    /// </summary>
    public required string CurrentUICulture { get; init; }


    /// <summary>
    /// The supported cultures of the application.
    /// </summary>
    public required LocalizationInfo[] SupportedCultures { get; init; }

    /// <summary>
    /// The supported UI cultures of the application.
    /// </summary>
    public required LocalizationInfo[] SupportedUICultures { get; init; }
}

/// <summary>
/// Represents a language and a language value for localization selection and similar purpose.
/// </summary>
/// <param name="Value">The CultureInfo in form de-DE or LanguageInfo in form de.</param>
/// <param name="NativeDisplayName">The native language display name</param>
public record LocalizationInfo(string Value, string NativeDisplayName);