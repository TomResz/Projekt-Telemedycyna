using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using PressureLogger.Infrastructure.DAL;

namespace PressureLogger.Infrastructure;

public static class Extensions
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        var connectionString = configuration["Sqlite"] ?? throw new ArgumentNullException();
        services.AddDbContext<PressureLoggerContext>(options =>
            options.UseSqlite(connectionString));

        return services;
    }
}
