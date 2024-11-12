using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;

namespace PressureLogger.Infrastructure.MQTT;

internal sealed class MqttOptionsConfiguration(IConfiguration configuration) : IConfigureNamedOptions<MqttOptions>
{
    private readonly IConfiguration _configuration = configuration;

    public void Configure(MqttOptions options)
    {
        bool isDocker = Environment.GetEnvironmentVariable("DOTNET_RUNNING_IN_CONTAINER") == "true";
        if (isDocker)
        {
            _configuration.GetSection("Mqtt:Docker").Bind(options);
        }
        else
        {
            _configuration.GetSection("Mqtt:Windows").Bind(options);
        }
    }
    
    public void Configure(string? name, MqttOptions options)
    {
        Configure(options);
    }
}