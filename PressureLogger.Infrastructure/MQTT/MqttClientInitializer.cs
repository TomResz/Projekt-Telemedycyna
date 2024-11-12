using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

namespace PressureLogger.Infrastructure.MQTT;

public static class MqttClientInitializer
{
    public static async Task Start(IServiceProvider provider)
    {
        using var scope = provider.CreateScope();

        var service = scope.ServiceProvider.GetRequiredService<IMqttService>();
        var topic = scope.ServiceProvider.GetRequiredService<IOptions<MqttOptions>>().Value.Topic!;
        
        await service.ConnectAsync();
        await service.SubscribeAsync(topic);
    }
}