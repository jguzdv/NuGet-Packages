using JGUZDV.ClientStorage;

using Microsoft.JSInterop;

namespace JGUZDV.ClientStorage.Defaults;

public class BlazorLifeCycleEvents : ILifeCycleEvents
{
    private readonly IJSRuntime _jsRuntime;

    public BlazorLifeCycleEvents(IJSRuntime jsRuntime)
    {
        _jsRuntime = jsRuntime;
        _ = Init();
    }
    private async Task Init()
    {
        var module = await _jsRuntime.InvokeAsync<IJSObjectReference>("import", "_content/JGUZDV.ClientStorage.Blazor/js/LifeCycleEvents.js");

        var dotNetRef = DotNetObjectReference.Create(this);
        await module.InvokeVoidAsync("addLifeCycleEvents", dotNetRef);
    }

    [JSInvokable]
    public void TriggerStopped()
    {
        Stopped?.Invoke(null, new());
    }

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