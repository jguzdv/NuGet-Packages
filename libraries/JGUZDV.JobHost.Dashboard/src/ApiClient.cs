using System.Net.Http.Json;

using JGUZDV.JobHost.Shared;
using JGUZDV.JobHost.Shared.Model;

namespace JGUZDV.JobHost.Dashboard
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
        public Task ExecuteNow(int jobId, CancellationToken ct)
        {
            return _httpClient.PostAsync(Routes.ExecuteNow(jobId), null, ct);
        }

        /// <inheritdoc/>
        public async Task<JobCollection> GetJobs(CancellationToken ct)
        {
            return await _httpClient.GetFromJsonAsync<JobCollection>(Routes.GetJobs, ct) ?? new JobCollection { 
                Hosts = new Dictionary<int, Shared.Model.Host>(),
                JobsByHost = new Dictionary<int,List<Job>>()
            };
        }
        
    }
}
