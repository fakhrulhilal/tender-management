using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Serilog;
using Serilog.Sinks.Elasticsearch;
using System.Reflection;
using Microsoft.Extensions.Logging;
using TenderManagement.Application.Common.Port;
using TenderManagement.Infrastructure.Identity;
using TenderManagement.Infrastructure.Persistence;
using TenderManagement.Infrastructure.Service;

namespace TenderManagement.Infrastructure
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
        {
            if (configuration.GetValue<bool>("UseInMemoryDatabase"))
            {
                services.AddDbContext<ApplicationDbContext>(options =>
                    options.UseInMemoryDatabase("TenderManagementDb"));
            }
            else
            {
                services.AddDbContext<ApplicationDbContext>(options =>
                    options.UseSqlServer(
                        configuration.GetConnectionString("DefaultConnection"),
                        b => b.MigrationsAssembly(typeof(ApplicationDbContext).Assembly.FullName)));
            }

            var cacheConfig = configuration.GetSection(CacheConfig.Key).Get<CacheConfig>();
            if (cacheConfig.UseRedis)
            {
                services.AddStackExchangeRedisCache(options => options.Configuration = cacheConfig.RedisConnection);
            }
            else
            {
                services.AddDistributedMemoryCache();
            }
            services.AddScoped<IApplicationDbContext>(provider => provider.GetService<ApplicationDbContext>());

            services.AddScoped<IDomainEventService, DomainEventService>();

            services
                .AddDefaultIdentity<ApplicationUser>()
                .AddRoles<IdentityRole>()
                .AddEntityFrameworkStores<ApplicationDbContext>();

            services.AddIdentityServer()
                .AddApiAuthorization<ApplicationUser, ApplicationDbContext>();

            services.AddTransient<IDateTime, DateTimeService>();
            services.AddTransient<IIdentityService, IdentityService>();

            services.AddAuthentication()
                .AddIdentityServerJwt();

            services.AddAuthorization(options =>
            {
                options.AddPolicy("CanPurge", policy => policy.RequireRole("Administrator"));
            });

            return services;
        }

        public static IHostBuilder UseInfrastructure(this IHostBuilder host)
        {
            host.ConfigureLogging(log => log.ClearProviders());
            host.UseSerilog((context, logConfig) =>
            {
                var config = context.Configuration;
                var esUri = new System.Uri(config.GetValue<string>("AppLog:ESUri"));
                string appName = config.GetValue<string>("AppLog:Name") ?? Assembly.GetCallingAssembly().FullName ?? Assembly.GetExecutingAssembly().FullName; 
                string envName = context.HostingEnvironment.EnvironmentName?.ToLower().Replace(".", "_");
                logConfig
                    .Enrich.FromLogContext()
                    .Enrich.WithMachineName()
                    .Enrich.WithProperty("Environment", context.HostingEnvironment.EnvironmentName)
                    .ReadFrom.Configuration(context.Configuration)
                    .WriteTo.Elasticsearch(new ElasticsearchSinkOptions(esUri)
                    {
                        IndexFormat = $"{appName}-logs-{envName}-{System.DateTime.UtcNow:yyyy-MM}",
                        AutoRegisterTemplate = true
                    });
            });

            return host;
        }
    }
}