using Microsoft.Extensions.Logging;
using System.Net.Http.Json;
using System.Text.Json;

namespace JGUZDV.Blazor.Components.NavigatorSwitch
{
    public class NavigatorSwitchHttpClient
    {
        private readonly ILogger<NavigatorSwitchHttpClient> _logger;
        private readonly HttpClient _http;
        private readonly string _apiBasePath = "api/v1/";

        protected JsonSerializerOptions _jsonOptions = new JsonSerializerOptions()
        {
            PropertyNameCaseInsensitive = true
        };

        public NavigatorSwitchHttpClient(HttpClient http, ILogger<NavigatorSwitchHttpClient> logger)
        {
            _http = http;
            _logger = logger;
            _jsonOptions.Converters.Add(new System.Text.Json.Serialization.JsonStringEnumConverter());
        }

        public async Task<List<WidgetEntry>?> FetchNavigatorSwitchServices()
        {
            try
            {
                var response = await _http.GetFromJsonAsync<List<WidgetEntry>>(
                    _apiBasePath + "services/for/widget",
                    _jsonOptions);

                return response;
            }
            catch(HttpRequestException e)
            {
                _logger.LogError(e, "Could not fetch service list.");
                return null;
            }
        }
    }
}
