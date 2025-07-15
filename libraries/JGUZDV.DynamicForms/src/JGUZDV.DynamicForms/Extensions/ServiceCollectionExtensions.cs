using JGUZDV.DynamicForms.Model;

using Microsoft.Extensions.DependencyInjection;

namespace JGUZDV.DynamicForms.Extensions;

/// <summary>
/// Extensions to add dynamic forms to the service collection.
/// </summary>
public static class ServiceCollectionExtensions
{
    /// <summary>
    /// Adds dynamic forms services to the service collection.
    /// </summary>
    /// <param name="services"></param>
    /// <returns></returns>
    public static DynamicFormsBuilder AddDynamicForms(this IServiceCollection services)
    {
        return new DynamicFormsBuilder(services);
    }
}
