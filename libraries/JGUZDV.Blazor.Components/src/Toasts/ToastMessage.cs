using System;
using System.Threading;

namespace JGUZDV.Blazor.Components.Toasts
{
    public class ToastMessage : IDisposable
    {
        private readonly IToastService _toastService;
        private readonly Timer? _timer;

        public ToastMessage(ToastLevel toastLevel, string message, string? title, TimeSpan? timeout, IToastService toastService)
        {
            ToastLevel = toastLevel;
            Message = message;
            Title = title;

            _toastService = toastService;

            if(timeout != null)
            {
                _timer = new Timer((_) => Hide(), null, timeout.Value, Timeout.InfiniteTimeSpan);
            }
        }

        public ToastLevel ToastLevel { get; }
        public string Message { get; }
        public string? Title { get; }

        public void Dispose()
        {
            _timer?.Dispose();
        }

        public void Hide()
        {
            _toastService.Hide(this);
        }
    }
}
