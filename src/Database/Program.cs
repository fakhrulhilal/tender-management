using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using TenderManagement.Database;
using Yuniql.Extensibility;

using var host = Host.CreateDefaultBuilder(args)
    .ConfigureAppConfiguration(config =>
    {
        config.AddEnvironmentVariables(prefix: "DB_");
        config.AddUserSecrets<Migration>(true);
    })
    .ConfigureServices(services =>
    {
        services.AddTransient<ITraceService, ConsoleTraceService>();
        services.AddSingleton<IHostedService, Migration>();
    }).Build();
await host.StartAsync();
