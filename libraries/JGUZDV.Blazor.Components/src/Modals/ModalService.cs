using Microsoft.Extensions.Logging;

namespace JGUZDV.Blazor.Components.Modals
{
    internal class ModalService
    {
        public IEnumerable<IModal> Modals => _modals.Select(x => x.Modal);

        public event EventHandler<IModal?>? ModalChanged;
        private readonly List<ModalCompletionSource> _modals = new();

        private readonly ILogger<ModalService> _logger;

        public ModalService(ILogger<ModalService> logger)
        {
            _logger = logger;
        }

        public Task<ModalResult> ShowModal(IModal modal)
        {
            _modals.Add(new(modal));

            ModalChanged?.Invoke(this, modal);
            return _modals.Last().TaskCompletionSource.Task;
        }

        public void Close(IModal modal, ModalResult result)
        {
            if(!_modals.Any())
            {
                _logger.LogError($"No modal has been opened, but was attempted to be closed");
                return;
            }

            if (_modals[_modals.Count - 1].Modal != modal)
            {
                _logger.LogError("The topmost modal was not the one that was requested to be closed.");
                return;
            }

            _modals[_modals.Count - 1].TaskCompletionSource.SetResult(result);
            _modals.RemoveAt(_modals.Count - 1);

            ModalChanged?.Invoke(this, _modals.LastOrDefault()?.Modal);
        }

        private record ModalCompletionSource(IModal Modal)
        {
            public TaskCompletionSource<ModalResult> TaskCompletionSource { get; } = new();
        }
    }
}
