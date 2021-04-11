using System.Reflection;
using FluentValidation;
using Imprise.MediatR.Extensions.Caching;
using MediatR;
using MediatR.Pipeline;
using Microsoft.Extensions.DependencyInjection;
using TenderManagement.Application.Common.Behaviour;

namespace TenderManagement.Application
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddApplication(this IServiceCollection services)
        {
            services.AddAutoMapper(Assembly.GetExecutingAssembly(), typeof(DependencyInjection).Assembly);
            services.AddValidatorsFromAssembly(typeof(DependencyInjection).Assembly);
            services.AddMediatR(Assembly.GetExecutingAssembly(), typeof(DependencyInjection).Assembly);
            services.AddTransient(typeof(IPipelineBehavior<,>), typeof(UnhandledExceptionBehaviour<,>));
            services.AddTransient(typeof(IPipelineBehavior<,>), typeof(AuthorizationBehaviour<,>));
            services.AddTransient(typeof(IPipelineBehavior<,>), typeof(ValidationBehaviour<,>));
            services.AddTransient(typeof(IPipelineBehavior<,>), typeof(PerformanceBehaviour<,>));

            // Configure MediatR Pipeline with cache invalidation and cached request behaviors
            services.AddScoped(typeof(IPipelineBehavior<,>), typeof(RequestPreProcessorBehavior<,>));
            services.AddScoped(typeof(IPipelineBehavior<,>), typeof(CacheInvalidationBehavior<,>));
            services.AddScoped(typeof(IPipelineBehavior<,>), typeof(CacheBehavior<,>));
            services.AddScoped(typeof(IPipelineBehavior<,>), typeof(RequestPostProcessorBehavior<,>));

            // Use Scrutor to scan and register all classes as their implemented interfaces.
            // This simplifies hooking up any ICache<Request, Response> implementation for the pipeline
            services.Scan(scan => scan
                .FromAssembliesOf(typeof(IMediator), typeof(DependencyInjection))
                .AddClasses()
                .AsImplementedInterfaces());

            return services;
        }
    }
}
