namespace JGUZDV.Blazor.Components.Toasts;

/// <summary>
/// Obsolete: Use ToastMessages component as a ref with .Show() instead.
/// </summary>
[Obsolete("Use ToastMessages component as a ref with .Show() instead.")]
public interface IToastService
{
    /// <summary>
    /// Obsolete: Use ToastMessages component as a ref with .Show() instead.
    /// </summary>
    IEnumerable<ToastMessage> GetToasts();

    /// <summary>
    /// Obsolete: Use ToastMessages component as a ref with .Show() instead.
    /// </summary>
    event EventHandler ToastsChanged;

    /// <summary>
    /// Obsolete: Use ToastMessages component as a ref with .Show() instead.
    /// </summary>
    ToastMessage Show(ToastLevel toastLevel, string message, string? title);

    /// <summary>
    /// Obsolete: Use ToastMessages component as a ref with .Show() instead.
    /// </summary>
    ToastMessage Show(ToastLevel toastLevel, string message, string? title, int timoutSeconds)
        => Show(toastLevel, message, title, TimeSpan.FromSeconds(timoutSeconds));

    /// <summary>
    /// Obsolete: Use ToastMessages component as a ref with .Show() instead.
    /// </summary>
    ToastMessage Show(ToastLevel toastLevel, string message, string? title, TimeSpan timeout);

    /// <summary>
    /// Obsolete: Use ToastMessages component as a ref with .Show() instead.
    /// </summary>
    void Hide(ToastMessage toast);

    /// <summary>
    /// Obsolete: Use ToastMessages component as a ref with .Show() instead.
    /// </summary>
    void HideAll();
}
