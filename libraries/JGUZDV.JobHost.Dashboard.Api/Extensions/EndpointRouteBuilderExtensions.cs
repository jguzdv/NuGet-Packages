using JGUZDV.JobHost.Dashboard.Services;

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
        /// <param name="routePrefix"></param>
        /// <returns></returns>
        public static RouteGroupBuilder MapJobHostDashboardApi(this IEndpointRouteBuilder builder, string routePrefix)
        {
            var group = builder.MapGroup(routePrefix);

            group.MapGet(Routes.GetJobs, async (IDashboardService service) =>
            {
                var result = await service.GetJobs();

                return Results.Ok(result);
            });

            group.MapPost(Routes.ExecuteNowTemplate, async (int jobId, IDashboardService service) =>
            {
                await service.ExecuteNow(jobId);
            });

            return group;
        }
    }
}
