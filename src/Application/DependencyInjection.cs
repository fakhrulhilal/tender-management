using System;
using System.Collections.Generic;
using FluentValidation;
using Imprise.MediatR.Extensions.Caching;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using System.Linq;
using System.Reflection;
using TenderManagement.Application.Common.Behaviour;

namespace TenderManagement.Application
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddApplication(this IServiceCollection services)
        {
            services.AddAutoMapper(Assembly.GetExecutingAssembly());
            services.AddValidatorsFromAssembly(Assembly.GetExecutingAssembly());
            services.AddMediatR(Assembly.GetExecutingAssembly());
            services.AddTransient(typeof(IPipelineBehavior<,>), typeof(UnhandledExceptionBehaviour<,>));
            services.AddTransient(typeof(IPipelineBehavior<,>), typeof(AuthorizationBehaviour<,>));
            services.AddTransient(typeof(IPipelineBehavior<,>), typeof(ValidationBehaviour<,>));
            services.AddTransient(typeof(IPipelineBehavior<,>), typeof(PerformanceBehaviour<,>));

            // Configure MediatR Pipeline with cache invalidation and cached request behaviors
            services.AddScoped(typeof(IPipelineBehavior<,>), typeof(CacheInvalidationBehavior<,>));
            services.AddScoped(typeof(IPipelineBehavior<,>), typeof(CacheBehavior<,>));
            foreach (var (impl, contract) in GetImplementations(typeof(ICache<,>), typeof(ICacheInvalidator<>)))
                services.AddScoped(contract, impl);

            return services;
        }

        public static IEnumerable<(Type Implementation, Type Contract)> GetImplementations(params Type[] contracts)
        {
            var implementations = Assembly.GetExecutingAssembly().GetExportedTypes()
                .Where(type => type.IsClass && !type.IsAbstract)
                .Where(type =>
                    type.GetInterfaces().Any(i => i.IsGenericType && contracts.Contains(i.GetGenericTypeDefinition())))
                .Distinct().ToArray();
            foreach (var impl in implementations)
                foreach (var contract in impl.GetInterfaces())
                    yield return (impl, contract);
        }
    }
}
