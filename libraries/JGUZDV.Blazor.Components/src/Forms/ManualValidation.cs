using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;

using System.Diagnostics.CodeAnalysis;

namespace JGUZDV.Blazor.Components.Forms;

/// <summary>
/// A component that provides access to the <see cref="ValidationMessageStore"/> of the current <see cref="EditContext"/>.
/// </summary>
[Obsolete("Use ValidationStoreAccessor instead.")]
public class ManualValidation : ValidationStoreAccessor { }

/// <summary>
/// A component that provides access to the <see cref="ValidationMessageStore"/> of the current <see cref="EditContext"/>.
/// </summary>
public class ValidationStoreAccessor : ComponentBase
{
    [NotNull]
    private ValidationMessageStore? _messageStore = default!;

    [CascadingParameter, NotNull]
    private EditContext? CurrentEditContext { get; set; }

    /// <inheritdoc />
    protected override void OnInitialized()
    {
        if (CurrentEditContext == null)
        {
            throw new InvalidOperationException(
                $"{nameof(ValidationStoreAccessor)} requires a cascading " +
                $"parameter of type {nameof(EditContext)}. " +
                $"For example, you can use {nameof(ValidationStoreAccessor)} " +
                $"inside an {nameof(EditForm)}.");
        }

        _messageStore = new(CurrentEditContext);

        CurrentEditContext.OnValidationRequested += (s, e) =>
            _messageStore.Clear();
        CurrentEditContext.OnFieldChanged += (s, e) =>
            _messageStore.Clear(e.FieldIdentifier);
    }

    /// <summary>
    /// Add an error message to the specified field.
    /// </summary>
    public void AddError(FieldIdentifier field, string message)
    {
        _messageStore.Add(field, message);
        CurrentEditContext.NotifyValidationStateChanged();
    }

    /// <summary>
    /// Add an error message to the specified field.
    /// </summary>
    public void AddErrors(Dictionary<string, List<string>> errors)
    {
        foreach (var err in errors)
        {
            _messageStore.Add(CurrentEditContext.Field(err.Key), err.Value);
        }

        CurrentEditContext.NotifyValidationStateChanged();
    }

    /// <summary>
    /// Clear all error messages.
    /// </summary>
    public void ClearErrors()
    {
        _messageStore.Clear();
        CurrentEditContext.NotifyValidationStateChanged();
    }
}

