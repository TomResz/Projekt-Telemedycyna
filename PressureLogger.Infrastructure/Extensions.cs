using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using MQTTnet.Channel;
using PressureLogger.Infrastructure.DAL;
using PressureLogger.Infrastructure.MQTT;

namespace PressureLogger.Infrastructure;

public static class Extensions
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        var connectionString = configuration["Sqlite"] ?? throw new ArgumentNullException();
        services.AddDbContext<PressureLoggerContext>(options =>
            options.UseSqlite(connectionString));
        
        services.ConfigureOptions<MqttOptionsConfiguration>();
        
        services.AddSingleton<IMqttService,MqttService>();
        
        return services;
    }
}
