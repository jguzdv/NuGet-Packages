namespace JGUZDV.Blazor.Components.Toasts
{
    [Obsolete("Use ToastMessages component as a ref with .Show() instead.")]
    public class ToastService : IToastService
    {
        private readonly List<ToastMessage> _toasts = new();
        public IEnumerable<ToastMessage> GetToasts() => _toasts;

        public event EventHandler? ToastsChanged;
        public void RaiseToastsChanged()
        {
            ToastsChanged?.Invoke(this, EventArgs.Empty);
        }

        public ToastMessage Show(ToastLevel toastLevel, string message, string? title)
        {
            var toastMessage = new ToastMessage(toastLevel, message, title, null, this);
            _toasts.Add(toastMessage);
            RaiseToastsChanged();

            return toastMessage;
        }

        public ToastMessage Show(ToastLevel toastLevel, string message, string? title, TimeSpan timeout)
        {
            var toastMessage = new ToastMessage(toastLevel, message, title, timeout, this);
            _toasts.Add(toastMessage);
            RaiseToastsChanged();

            return toastMessage;
        }

        public void Hide(ToastMessage toast)
        {
            if (_toasts.Remove(toast))
            {
                RaiseToastsChanged();
            }
        }

        public void HideAll() {
            _toasts.Clear();
            RaiseToastsChanged();
        }
    }
}
