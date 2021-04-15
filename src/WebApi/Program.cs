using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System.Threading.Tasks;
using TenderManagement.Infrastructure;

namespace TenderManagement.WebApi
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            var host = CreateHostBuilder(args).Build();
            await host.RunAsync();
        }

        public static IHostBuilder CreateHostBuilder(string[] args) => Host
            .CreateDefaultBuilder(args)
            .UseInfrastructure()
            .ConfigureWebHostDefaults(builder =>
            {
                builder.UseStartup<Startup>();
                builder.UseDefaultServiceProvider(opt => opt.ValidateScopes = false);
            })
            .ConfigureServices(services =>
            {
                services.AddSingleton<Yuniql.Extensibility.ITraceService, Database.LogTraceService>();
                services.AddSingleton<IHostedService, Database.Migration>();
                services.AddSingleton<IHostedService, Infrastructure.Service.Migration>();
            });
    }
}
