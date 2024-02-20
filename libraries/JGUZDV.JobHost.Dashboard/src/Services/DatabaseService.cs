using JGUZDV.JobHost.Dashboard.Model;
using JGUZDV.JobHost.Database;

namespace JGUZDV.JobHost.Dashboard.Services
{
    public class DatabaseService : IDashboardService
    {
        private readonly JobHostContext _dbContext;

        public DatabaseService(JobHostContext dbContext)
        {
            _dbContext = dbContext;
        }

        public Task<JobCollection> GetJobs()
        {
            throw new NotImplementedException();
        }
    }
}
