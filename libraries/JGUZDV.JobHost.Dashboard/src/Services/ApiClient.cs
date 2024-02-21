using JGUZDV.JobHost.Dashboard.Model;

namespace JGUZDV.JobHost.Dashboard.Services
{
    public class ApiClient : IDashboardService
    {
        private readonly HttpClient _httpClient;

        public ApiClient(HttpClient client, string? baseAddress = null)
        {
            if (!string.IsNullOrWhiteSpace(baseAddress))
            {
                client.BaseAddress = new Uri(baseAddress);
            }

            _httpClient = client;
        }

        public Task ExecuteNow(int jobId)
        {
            return _httpClient.PostAsync(Routes.ExecuteNow(jobId), null);
        }

        public Task<JobCollection> GetJobs()
        {
            return _httpClient.GetFromJsonAsync<JobCollection>(Routes.GetJobs)!;
        }
    }
}
