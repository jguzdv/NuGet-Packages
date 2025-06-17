namespace JGUZDV.Blazor.Components.Toasts
{
    /// <summary>
    /// Obsolete: Use ToastMessages component as a ref with .Show() instead.
    /// </summary>
    [Obsolete("Use ToastMessages component as a ref with .Show() instead.")]
    public class ToastService : IToastService
    {
        private readonly List<ToastMessage> _toasts = new();

        /// <summary>
        /// Obsolete: Use ToastMessages component as a ref with .Show() instead.
        /// </summary>
        [Obsolete("Use ToastMessages component as a ref with .Show() instead.")]
        public IEnumerable<ToastMessage> GetToasts() => _toasts;

        /// <summary>
        /// Obsolete: Use ToastMessages component as a ref with .Show() instead.
        /// </summary>
        [Obsolete("Use ToastMessages component as a ref with .Show() instead.")]
        public event EventHandler? ToastsChanged;

        /// <summary>
        /// Obsolete: Use ToastMessages component as a ref with .Show() instead.
        /// </summary>
        [Obsolete("Use ToastMessages component as a ref with .Show() instead.")]
        public void RaiseToastsChanged()
        {
            ToastsChanged?.Invoke(this, EventArgs.Empty);
        }

        /// <summary>
        /// Obsolete: Use ToastMessages component as a ref with .Show() instead.
        /// </summary>
        [Obsolete("Use ToastMessages component as a ref with .Show() instead.")]
        public ToastMessage Show(ToastLevel toastLevel, string message, string? title)
        {
            var toastMessage = new ToastMessage(toastLevel, message, title, null, this);
            _toasts.Add(toastMessage);
            RaiseToastsChanged();

            return toastMessage;
        }

        /// <summary>
        /// Obsolete: Use ToastMessages component as a ref with .Show() instead.
        /// </summary>
        [Obsolete("Use ToastMessages component as a ref with .Show() instead.")]
        public ToastMessage Show(ToastLevel toastLevel, string message, string? title, TimeSpan timeout)
        {
            var toastMessage = new ToastMessage(toastLevel, message, title, timeout, this);
            _toasts.Add(toastMessage);
            RaiseToastsChanged();

            return toastMessage;
        }

        /// <summary>
        /// Obsolete: Use ToastMessages component as a ref with .Show() instead.
        /// </summary>
        [Obsolete("Use ToastMessages component as a ref with .Show() instead.")]
        public void Hide(ToastMessage toast)
        {
            if (_toasts.Remove(toast))
            {
                RaiseToastsChanged();
            }
        }

        /// <summary>
        /// Obsolete: Use ToastMessages component as a ref with .Show() instead.
        /// </summary>
        [Obsolete("Use ToastMessages component as a ref with .Show() instead.")]
        public void HideAll() {
            _toasts.Clear();
            RaiseToastsChanged();
        }
    }
}
