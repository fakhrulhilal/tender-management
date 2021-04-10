using FluentValidation;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using System.Reflection;
using Imprise.MediatR.Extensions.Caching;
using MediatR.Pipeline;
using TenderManagement.Application.Common.Behaviour;
using TenderManagement.Application.Tender;

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
            services.AddScoped(typeof(IPipelineBehavior<,>), typeof(RequestPreProcessorBehavior<,>));
            services.AddScoped(typeof(IPipelineBehavior<,>), typeof(CacheInvalidationBehavior<,>));
            services.AddScoped(typeof(IPipelineBehavior<,>), typeof(CacheBehavior<,>));
            services.AddScoped(typeof(IPipelineBehavior<,>), typeof(RequestPostProcessorBehavior<,>));

            // Use Scrutor to scan and register all classes as their implemented interfaces.
            // This simplifies hooking up any ICache<Request, Response> implementation for the pipeline
            services.Scan(scan => scan
                .FromAssembliesOf(typeof(IMediator), typeof(BaseTenderEntity))
                .AddClasses()
                .AsImplementedInterfaces());

            return services;
        }
    }
}
