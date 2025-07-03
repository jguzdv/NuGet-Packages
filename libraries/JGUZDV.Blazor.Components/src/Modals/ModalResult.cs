namespace JGUZDV.Blazor.Components.Modals;

/// <summary>
/// Specifies the result of a modal dialog interaction.
/// </summary>
public enum ModalResult
{
    /// <summary>
    /// Undefined is the default value, indicating that is has been aborted and we could not determine the result of the modal dialog interaction.
    /// </summary>
    Undefined,

    /// <summary>
    /// The dialog has been dismissed, typically by the user clicking a close button or outside the modal area.
    /// </summary>
    Dismissed,

    /// <summary>
    /// The dialog has been accepted, typically by the user clicking an "OK" or "Accept" button.
    /// </summary>
    Accepted
};