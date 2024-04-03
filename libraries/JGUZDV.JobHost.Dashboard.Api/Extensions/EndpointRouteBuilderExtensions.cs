using JGUZDV.JobHost.Dashboard.Shared;
using JGUZDV.JobHost.Shared;

namespace JGUZDV.JobHost.Dashboard.Extensions
{
    /// <summary>
    /// Extension class for endpoint route builder
    /// </summary>
    public static class EndpointRouteBuilderExtensions
    {
        /// <summary>
        /// Maps endpoints required by the dashboard
        /// </summary>
        /// <param name="builder"></param>
        /// <returns></returns>
        public static IEndpointRouteBuilder MapJobHostDashboardApi(this IEndpointRouteBuilder builder)
        {
            builder.MapGet(Routes.GetJobs, async (IDashboardService service, HttpContext context) =>
            {
                var result = await service.GetJobs(context.RequestAborted);

                return Results.Ok(result);
            });

            builder.MapPost(Routes.ExecuteNowTemplate, async (int jobId, IDashboardService service, HttpContext context) =>
            {
                await service.ExecuteNow(jobId, context.RequestAborted);
            });

            return builder;
        }
    }
}
