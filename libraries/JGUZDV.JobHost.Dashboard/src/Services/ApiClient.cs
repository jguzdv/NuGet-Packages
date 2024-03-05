using System.Net.Http.Json;

using JGUZDV.JobHost.Dashboard.Model;

namespace JGUZDV.JobHost.Dashboard.Services
{
    public class ApiClient : IDashboardService
    {
        private readonly HttpClient _httpClient;


        public ApiClient(HttpClient client)
        {
            _httpClient = client;
        }

        /// <inheritdoc/>
        public Task ExecuteNow(int jobId)
        {
            return _httpClient.PostAsync(Routes.ExecuteNow(jobId), null);
        }

        /// <inheritdoc/>
        public async Task<JobCollection> GetSteveJobs()
        {
            return await _httpClient.GetFromJsonAsync<JobCollection>(Routes.GetJobs)!;
        }
        
    }
}
