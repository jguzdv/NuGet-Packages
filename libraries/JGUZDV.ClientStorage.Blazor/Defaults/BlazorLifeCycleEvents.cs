using Microsoft.JSInterop;

namespace JGUZDV.ClientStorage.Defaults;

/// <summary>
/// LifeCycleEvents using the javascript visibilitychange event
/// </summary>
public class BlazorLifeCycleEvents : ILifeCycleEvents
{
    private readonly IJSRuntime _jsRuntime;

    /// <summary>
    /// Constructor for <see cref="BlazorLifeCycleEvents"/>
    /// </summary>
    /// <param name="jsRuntime"></param>
    /// <exception cref="InvalidOperationException"></exception>
    public BlazorLifeCycleEvents(IJSRuntime jsRuntime)
    {
        _jsRuntime = jsRuntime;

        try
        {
            Init().Wait(); //TODO
        }
        catch (Exception e)
        {
            throw new InvalidOperationException($"Could not initialize {nameof(BlazorLifeCycleEvents)}", e);
        }
    }

    private async Task Init()
    {
        var module = await _jsRuntime.InvokeAsync<IJSObjectReference>("import", "./_content/JGUZDV.ClientStorage.Blazor/js/LifeCycleEvents.js");
        var dotNetRef = DotNetObjectReference.Create(this);
        await module.InvokeVoidAsync("addLifeCycleEvents", dotNetRef);
    }

    /// <summary>
    /// Trigger the <see cref="Stopped"/> event
    /// </summary>
    [JSInvokable]
    public void TriggerStopped()
    {
        Stopped?.Invoke(null, new());
    }

    /// <summary>
    /// Trigger the <see cref="Resumed"/> event
    /// </summary>
    [JSInvokable]
    public void TriggerResumed()
    {
        Resumed?.Invoke(null, new());
    }

    /// <inheritdoc/>
    public event EventHandler? Stopped;

    /// <inheritdoc/>
    public event EventHandler? Resumed;
}