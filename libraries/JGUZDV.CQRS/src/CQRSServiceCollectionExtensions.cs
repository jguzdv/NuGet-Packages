using JGUZDV.CQRS;
using JGUZDV.CQRS.Commands;
using JGUZDV.CQRS.Queries;
using System.Diagnostics;

namespace Microsoft.Extensions.DependencyInjection
{
    /// <summary>
    /// Extensions for adding CQRS handlers to the service collection.
    /// </summary>
    public static class CQRSServiceCollectionExtensions
    {
        /// <summary>
        /// Adds CQRS handlers (both commands and queries) to the service collection.
        /// </summary>
        public static IServiceCollection AddCQRSHandlers(
            this IServiceCollection services, params Type[] assemblyOfTypes)
        {
            return services
                .AddCommandHandlers(assemblyOfTypes)
                .AddQueryHandlers(assemblyOfTypes);
        }

        /// <summary>
        /// Registers command handler implementations in the specified assemblies with the dependency injection
        /// container.
        /// </summary>
        /// <remarks>This method scans the specified assemblies for classes that implement the <see
        /// cref="ICommandHandler{TCommand}"/> interface and registers them as scoped services in the dependency
        /// injection container. Classes marked with the <see cref="CQRSDecoratorAttribute"/> are excluded from
        /// registration.</remarks>
        /// <param name="services">The <see cref="IServiceCollection"/> to which the command handlers will be added.</param>
        /// <param name="assemblyOfTypes">An array of types used to identify the assemblies to scan for command handler implementations. The
        /// assemblies containing these types will be scanned.</param>
        /// <returns>The updated <see cref="IServiceCollection"/> instance.</returns>
        public static IServiceCollection AddCommandHandlers(
            this IServiceCollection services, params Type[] assemblyOfTypes)
        {
            services.Scan(
                s => s.FromAssembliesOf(assemblyOfTypes)
                    .AddClasses(classes =>
                        classes
                            .AssignableToAny(typeof(ICommandHandler<>))
                            .WithoutAttribute<CQRSDecoratorAttribute>()
                       ,
                       publicOnly: false)
                    .AsImplementedInterfaces()
                    .WithScopedLifetime()
                );

            return services;
        }

        /// <summary>
        /// Registers query handler implementations in the specified assemblies with the dependency injection
        /// container.
        /// </summary>
        /// <remarks>This method scans the specified assemblies for classes that implement the <see
        /// cref="IQueryHandler{TCommand}"/> interface and registers them as scoped services in the dependency
        /// injection container. Classes marked with the <see cref="CQRSDecoratorAttribute"/> are excluded from
        /// registration.</remarks>
        /// <param name="services">The <see cref="IServiceCollection"/> to which the query handlers will be added.</param>
        /// <param name="assemblyOfTypes">An array of types used to identify the assemblies to scan for command handler implementations. The
        /// assemblies containing these types will be scanned.</param>
        /// <returns>The updated <see cref="IServiceCollection"/> instance.</returns>
        public static IServiceCollection AddQueryHandlers(
            this IServiceCollection services, params Type[] assemblyOfTypes)
        {
            services.Scan(
                s => s.FromAssembliesOf(assemblyOfTypes)
                      .AddClasses(classes =>
                        classes
                            .AssignableToAny(typeof(IQueryHandler<>))
                            .WithoutAttribute<CQRSDecoratorAttribute>()
                       ,
                       publicOnly: false)
                    .AsImplementedInterfaces()
                    .WithScopedLifetime()
                );

            return services;
        }


        /// <summary>
        /// Adds the specified decorator types to all command handler services in the dependency injection container.
        /// </summary>
        /// <remarks>This method applies the specified decorators to all command handler services in the
        /// container.  Each type in <paramref name="decoratorTypes"/> must implement the appropriate decorator pattern 
        /// for the command handlers being decorated.</remarks>
        /// <param name="services">The <see cref="IServiceCollection"/> to which the decorators will be added.</param>
        /// <param name="decoratorTypes">An array of types representing the decorators to apply to the command handlers.</param>
        /// <returns>The updated <see cref="IServiceCollection"/> with the decorators added.</returns>
        public static IServiceCollection AddCommandHandlerDecorators(this IServiceCollection services,
            params Type[] decoratorTypes)
            => services.AddCommandHandlerDecorators((x) => true, decoratorTypes);

        /// <summary>
        /// Adds the specified decorator types to all command handler services in the dependency injection container.
        /// </summary>
        /// <remarks>This method applies the specified decorators to all command handler services in the
        /// container.  Each type in <paramref name="decoratorTypes"/> must implement the appropriate decorator pattern 
        /// for the command handlers being decorated.</remarks>
        /// <param name="services">The <see cref="IServiceCollection"/> to which the decorators will be added.</param>
        /// <param name="typeMatch">A predicate that determines whether a service descriptor should be decorated.  Only services matching this
        /// predicate will be decorated.</param>
        /// <param name="decoratorTypes">An array of decorator types to apply to the matching command handler services.  Each decorator type must be
        /// a generic type that can be constructed for the command handler's generic arguments.</param>
        /// <returns>The updated <see cref="IServiceCollection"/> with the decorators applied.</returns>
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

        /// <summary>
        /// Adds the specified decorator types to all query handlers in the service collection.
        /// </summary>
        /// <remarks>This method applies the specified decorators to all query handlers in the service
        /// collection. To filter which query handlers receive the decorators, use the overload that accepts a
        /// predicate.</remarks>
        /// <param name="services">The <see cref="IServiceCollection"/> to which the decorators will be added.</param>
        /// <param name="decoratorTypes">An array of <see cref="Type"/> objects representing the decorator types to apply to query handlers. Each
        /// type must implement a valid decorator pattern for query handlers.</param>
        /// <returns>The updated <see cref="IServiceCollection"/> with the decorators applied.</returns>
        public static IServiceCollection AddQueryHandlerDecorators(this IServiceCollection services,
            params Type[] decoratorTypes)
            => services.AddQueryHandlerDecorators((x) => true, decoratorTypes);



        /// <summary>
        /// Adds the specified decorator types to all query handlers in the service collection.
        /// </summary>
        /// <remarks>This method applies the specified decorators to all query handlers in the service
        /// collection. To filter which query handlers receive the decorators, use the overload that accepts a
        /// predicate.</remarks>
        /// <param name="services">The <see cref="IServiceCollection"/> to which the decorators will be added.</param>
        /// <param name="typeMatch">A predicate that determines whether a service descriptor should be decorated.  Only services matching this
        /// predicate will be decorated.</param>
        /// <param name="decoratorTypes">An array of <see cref="Type"/> objects representing the decorator types to apply to query handlers. Each
        /// type must implement a valid decorator pattern for query handlers.</param>
        /// <returns>The updated <see cref="IServiceCollection"/> with the decorators applied.</returns>
        public static IServiceCollection AddQueryHandlerDecorators(this IServiceCollection services,
            Predicate<ServiceDescriptor> typeMatch, params Type[] decoratorTypes)
        {
            foreach (var type in decoratorTypes)
                services.AddScoped(type);

            foreach (var s in services.Where(s => s.ServiceType.IsGenericType).ToList())
            {
                if (s.ServiceType.GetGenericTypeDefinition() != typeof(IQueryHandler<>))
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
