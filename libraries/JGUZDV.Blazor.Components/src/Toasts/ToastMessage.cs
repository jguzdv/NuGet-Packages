namespace JGUZDV.Blazor.Components.Toasts
{
    public class ToastMessage
    {
        private readonly IToastService _toastService;

        [Obsolete("Timeouts will not be supported anymore.")]
        public ToastMessage(ToastLevel toastLevel, string message, string? title, TimeSpan? timeout, IToastService toastService)
        {
            ToastLevel = toastLevel;
            Message = message;
            Title = title;

            _toastService = toastService;
        }

        
        public ToastMessage(ToastLevel toastLevel, string message, string? title)
        {
            ToastLevel = toastLevel;
            Message = message;
            Title = title;
        }

        public ToastLevel ToastLevel { get; }
        public string Message { get; }
        public string? Title { get; }

        public void Hide()
        {
            _toastService?.Hide(this);
        }
    }
}
