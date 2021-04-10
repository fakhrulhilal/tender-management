using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using TenderManagement.Infrastructure.Identity;
using TenderManagement.Infrastructure.Persistence;
using TenderManagement.WebUI;

var host = Host
    .CreateDefaultBuilder(args)
    .ConfigureWebHostDefaults(builder => builder.UseStartup<Startup>())
    .Build();

using var scope = host.Services.CreateScope();
var services = scope.ServiceProvider;
try
{
    var context = services.GetRequiredService<ApplicationDbContext>();
    if (context.Database.IsSqlServer()) context.Database.Migrate();

    var userManager = services.GetRequiredService<UserManager<ApplicationUser>>();
    var roleManager = services.GetRequiredService<RoleManager<IdentityRole>>();

    await ApplicationDbContextSeed.SeedDefaultUserAsync(userManager, roleManager);
    await ApplicationDbContextSeed.SeedSampleDataAsync(context);
}
catch (Exception ex)
{
    var logger = scope.ServiceProvider.GetRequiredService<ILogger<Startup>>();
    logger.LogError(ex, "An error occurred while migrating or seeding the database.");
    throw;
}

await host.RunAsync();
