using JGUZDV.CQRS;
using JGUZDV.CQRS.Commands;
using JGUZDV.CQRS.Queries;
using System.Diagnostics;

namespace Microsoft.Extensions.DependencyInjection
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddCQRSHandlers(
            this IServiceCollection services, params Type[] assemblyOfTypes)
        {
            services.Scan(
                s => s.FromAssembliesOf(assemblyOfTypes)
                      .AddClasses(c =>
                        c.AssignableToAny(typeof(IQueryHandler<,>))
                            .WithoutAttribute<CQRSDecoratorAttribute>()
                      )
                      .AsImplementedInterfaces()
                      .WithScopedLifetime());

            services.Scan(
                s => s.FromAssembliesOf(assemblyOfTypes)
                      .AddClasses(c =>
                        c.AssignableToAny(typeof(ICommandHandler<>))
                            .WithoutAttribute<CQRSDecoratorAttribute>()
                       )
                      .AsImplementedInterfaces()
                      .WithScopedLifetime());

            return services;
        }

        public static IServiceCollection AddCommandHandlerDecorators(this IServiceCollection services,
            params Type[] decoratorTypes)
            => services.AddCommandHandlerDecorators((x) => true, decoratorTypes);

        public static IServiceCollection AddCommandHandlerDecorators(this IServiceCollection services,
            Predicate<ServiceDescriptor> typeMatch, params Type[] decoratorTypes)
        {
            foreach (var type in decoratorTypes)
                services.AddScoped(type);

            foreach (var s in services.Where(s => s.ServiceType.IsGenericType).ToList())
            {
                if (s.ServiceType.GetGenericTypeDefinition() != typeof(ICommandHandler<>))
                    continue;

                if (!typeMatch(s))
                    continue;

                var genericArguments = s.ServiceType.GetGenericArguments();

                foreach (var decoratorType in decoratorTypes.Select(x => x.MakeGenericType(genericArguments)))
                {
                    Debug.WriteLine($"Decorating {s.ServiceType} with {decoratorType}");
                    services.Decorate(s.ServiceType, decoratorType);
                }
            }

            return services;
        }

        public static IServiceCollection AddQueryHandlerDecorators(this IServiceCollection services,
            params Type[] decoratorTypes)
            => services.AddQueryHandlerDecorators((x) => true, decoratorTypes);

        public static IServiceCollection AddQueryHandlerDecorators(this IServiceCollection services,
            Predicate<ServiceDescriptor> typeMatch, params Type[] decoratorTypes)
        {
            foreach (var type in decoratorTypes)
                services.AddScoped(type);

            foreach (var s in services.Where(s => s.ServiceType.IsGenericType).ToList())
            {
                if (s.ServiceType.GetGenericTypeDefinition() != typeof(IQueryHandler<,>))
                    continue;

                if (!typeMatch(s))
                    continue;

                var genericArguments = s.ServiceType.GetGenericArguments();

                foreach (var decoratorType in decoratorTypes.Select(x => x.MakeGenericType(genericArguments)))
                    services.Decorate(s.ServiceType, decoratorType);
            }

            return services;
        }
    }
}
