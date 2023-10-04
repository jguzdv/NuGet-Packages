using Microsoft.Extensions.Logging;

namespace JGUZDV.Blazor.Components.Modals
{
    internal class ModalService
    {
        public IEnumerable<Modal> Modals => _modals.Select(x => x.Modal);

        public event EventHandler<Modal>? ModalChanged;
        private readonly List<(Modal Modal, TaskCompletionSource TaskCompletionSource)> _modals = new();

        
        private readonly ILogger<ModalService> _logger;

        public ModalService(ILogger<ModalService> logger)
        {
            _logger = logger;
        }

        public Task ShowModal(Modal modal)
        {
            _modals.Add((modal, new()));

            ModalChanged?.Invoke(this, modal);
            return _modals.Last().TaskCompletionSource.Task;
        }

        public void CloseModal(Modal modal)
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

            _modals[_modals.Count - 1].TaskCompletionSource.SetResult();
            _modals.RemoveAt(_modals.Count - 1);

            ModalChanged?.Invoke(this, _modals.LastOrDefault().Modal);
        }
    }
}
