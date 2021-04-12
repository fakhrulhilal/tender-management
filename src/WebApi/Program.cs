using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Threading.Tasks;
using TenderManagement.Infrastructure.Identity;
using TenderManagement.Infrastructure.Persistence;
using TenderManagement.WebApi.Services;

namespace TenderManagement.WebApi
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            var host = CreateHostBuilder(args).Build();
            await EnsureDatabaseMigrated(host);
            await host.RunAsync();
        }

        private static async Task EnsureDatabaseMigrated(IHost host)
        {
            using var scope = host.Services.CreateScope();
            var services = scope.ServiceProvider;
            try
            {
                var context = services.GetRequiredService<ApplicationDbContext>();
                if (context.Database.IsSqlServer()) context.Database.Migrate();

                var userManager = services.GetRequiredService<UserManager<ApplicationUser>>();
                var roleManager = services.GetRequiredService<RoleManager<IdentityRole>>();

                var dbMigrationLogger = services.GetRequiredService<ILogger<Database.Startup>>();
                Database.Startup.MigrateDatabase(context.Database.GetConnectionString(),
                    new LogTraceService(dbMigrationLogger));
                await ApplicationDbContextSeed.SeedDefaultUserAsync(userManager, roleManager);
                await ApplicationDbContextSeed.SeedSampleDataAsync(context);
            }
            catch (Exception ex)
            {
                var logger = scope.ServiceProvider.GetRequiredService<ILogger<Program>>();
                logger.LogError(ex, "An error occurred while migrating or seeding the database.");
                throw;
            }
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureWebHostDefaults(builder =>
                {
                    builder.UseStartup<Startup>();
                    builder.UseDefaultServiceProvider(opt => opt.ValidateScopes = false);
                });
    }
}
