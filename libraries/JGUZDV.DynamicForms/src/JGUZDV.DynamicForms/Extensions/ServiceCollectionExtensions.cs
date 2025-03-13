using JGUZDV.DynamicForms.Model;

using Microsoft.Extensions.DependencyInjection;

namespace JGUZDV.DynamicForms.Extensions;

public static class ServiceCollectionExtensions
{
    public static DynamicFormsBuilder AddDynamicForms(this IServiceCollection services)
    {
        return new DynamicFormsBuilder(services);
    }
}
