using JGUZDV.JobHost.Dashboard.Services;
using JGUZDV.JobHost.Shared.Model;

namespace JGUZDV.JobHost.Dashboard.Api
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
        public async Task<JobCollection> GetJobs()
        {
            return await _httpClient.GetFromJsonAsync<JobCollection>(Routes.GetJobs) ?? new JobCollection { 
                Hosts = new Dictionary<int, Shared.Model.Host>(),
                JobsByHost = new Dictionary<int,List<Job>>()
            };
        }
        
    }
}
