using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using JGUZDV.JobHost.Abstractions;

using Microsoft.EntityFrameworkCore;

namespace JGUZDV.JobHost.Database
{
    public class TestFactory : IJobExecutionReporterFactory
    {
        private IDbContextFactory<JobHostContext> _factory;

        public TestFactory(IDbContextFactory<JobHostContext> factory)
        {
            _factory = factory;
        }

        public IJobExecutionReporter Create()
        {
            return _factory.CreateDbContext();
        }

        public async Task<IJobExecutionReporter> CreateAsync()
        {
            return await _factory.CreateDbContextAsync();
        }
    }
}
