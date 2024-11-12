namespace PressureLogger.Infrastructure.MQTT;

public interface IMqttService
{
    Task ConnectAsync();
    Task SubscribeAsync(string topic);
}