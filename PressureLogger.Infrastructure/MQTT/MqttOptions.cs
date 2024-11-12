using System.Runtime;

namespace PressureLogger.Infrastructure.MQTT;

public class MqttOptions
{
    public string Host { get; set; }
    public int Port { get; set; }
    public string ClientId { get; set; }
    public string Topic { get; set; }
}