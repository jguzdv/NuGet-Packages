using JGUZDV.JobHost.Dashboard.Services;

namespace JGUZDV.JobHost.Dashboard.Extensions
{
    public static class EndpointRouteBuilderExtensions
    {
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
