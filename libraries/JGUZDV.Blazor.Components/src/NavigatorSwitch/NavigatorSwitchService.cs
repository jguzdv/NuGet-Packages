using Microsoft.Extensions.Logging;

namespace JGUZDV.Blazor.Components.NavigatorSwitch
{
    public class NavigatorSwitchService
    {
        private readonly ILogger<NavigatorSwitchService> _logger;
        private readonly NavigatorSwitchHttpClient _apiClient;

        public NavigatorSwitchService(NavigatorSwitchHttpClient apiClient,
            ILogger<NavigatorSwitchService> logger)
        {
            _apiClient = apiClient;
            _logger = logger;

        }

        public async Task<List<WidgetEntry>> FindWidgetEntries()
        {
            try
            {
                var response = await _apiClient.FetchNavigatorSwitchServices();

                if (response != null)
                {
                    return response;
                }
                else
                {
                    _logger.LogError("NavigatorWidget: Response null, returning empty list.");
                    return [];
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unexpected exception while loading widget entries.");
                return [];
            }
        }
    }
}
