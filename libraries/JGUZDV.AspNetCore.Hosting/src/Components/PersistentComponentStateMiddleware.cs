using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;

namespace JGUZDV.AspNetCore.Hosting.Components;

/// <summary>
/// This middleware will persist the current registered IPersistentComponentStateProvider data to the PersistentComponentState.
/// </summary>
public class PersistentComponentStateMiddleware
{
    private readonly RequestDelegate _next;

    /// <summary>
    /// Creates a new instance of the PersistentComponentStateMiddleware.
    /// </summary>
    public PersistentComponentStateMiddleware(RequestDelegate next)
    {
        _next = next;
    }

    /// <summary>
    /// Invokes the middleware. This will register a callback to persist the current registered IPersistentComponentStateProvider data.
    /// </summary>
    public async Task Invoke(HttpContext context)
    {
        var applicationState = context.RequestServices.GetService<PersistentComponentState>();

        PersistingComponentStateSubscription? subscription = null;
        if (applicationState != null)
        {
            subscription = applicationState.RegisterOnPersisting(() => OnPersistState(context, applicationState), RenderMode.InteractiveWebAssembly);
        }

        try
        {
            await _next(context);
        }
        finally
        {
            subscription?.Dispose();
        }
    }

    private async Task OnPersistState(HttpContext context, PersistentComponentState applicationState)
    {
        var stateProviders = context.RequestServices.GetServices<IPersistentComponentStateProvider>();

        foreach(var provider in stateProviders)
        {
            await provider.PersistStateAsync(applicationState);
        }
    }
}
