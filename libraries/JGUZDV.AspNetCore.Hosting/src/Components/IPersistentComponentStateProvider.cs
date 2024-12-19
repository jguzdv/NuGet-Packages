using Microsoft.AspNetCore.Components;

namespace JGUZDV.AspNetCore.Hosting.Components;

/// <summary>
/// Represents a provider for persistent component state.
/// </summary>
public interface IPersistentComponentStateProvider
{
    /// <summary>
    /// Persists the state of the component.
    /// </summary>
    Task PersistStateAsync(PersistentComponentState applicationState);
}