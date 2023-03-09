using System;
using System.Collections.Generic;

namespace JGUZDV.Blazor.Components.Toasts;

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
