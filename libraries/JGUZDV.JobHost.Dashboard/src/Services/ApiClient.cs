using System.Net.Http.Json;

using JGUZDV.JobHost.Dashboard.Model;

namespace JGUZDV.JobHost.Dashboard.Services
{
    /// <inheritdoc/>
    public class ApiClient : IDashboardService
    {
        private readonly HttpClient _httpClient;

        /// <summary>
        /// Ctor
        /// </summary>
        /// <param name="client"></param>
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
            return await _httpClient.GetFromJsonAsync<JobCollection>(Routes.GetJobs) ?? new JobCollection { 
                Hosts = new Dictionary<int,Host>(),
                JobsByHost = new Dictionary<int,List<Job>>()
            };
        }
        
    }
}
