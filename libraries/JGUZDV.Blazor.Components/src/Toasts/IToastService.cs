namespace JGUZDV.Blazor.Components.Toasts;

[Obsolete("Use ToastMessages component as a ref with .Show() instead.")]
public interface IToastService
{
    IEnumerable<ToastMessage> GetToasts();

    event EventHandler ToastsChanged;

    ToastMessage Show(ToastLevel toastLevel, string message, string? title);
    ToastMessage Show(ToastLevel toastLevel, string message, string? title, int timoutSeconds)
        => Show(toastLevel, message, title, TimeSpan.FromSeconds(timoutSeconds));
    ToastMessage Show(ToastLevel toastLevel, string message, string? title, TimeSpan timeout);

    void Hide(ToastMessage toast);
    void HideAll();
}
