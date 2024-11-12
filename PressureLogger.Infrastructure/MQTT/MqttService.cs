using System.Text;
using System.Text.Json;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using MQTTnet;
using PressureLogger.Infrastructure.DAL;
using PressureLogger.Infrastructure.Models;
using PressureLogger.Shared.Services;

namespace PressureLogger.Infrastructure.MQTT;

internal sealed class MqttService : IMqttService
{
    private readonly MqttOptions _options;
    private readonly IServiceProvider _serviceProvider;
    private readonly ILogger<MqttService> _logger;
    private readonly IMqttClient _mqttClient;

    public MqttService(
        IOptions<MqttOptions> options,
        IServiceProvider serviceProvider,
        ILogger<MqttService> logger)
    {
        _serviceProvider = serviceProvider;
        _logger = logger;
        _options = options.Value;
        var factory = new MqttClientFactory();
        _mqttClient = factory.CreateMqttClient();
        _mqttClient.ApplicationMessageReceivedAsync += OnMessageReceived;
    }
    public async Task ConnectAsync()
    {
        var options = new MqttClientOptionsBuilder()
            .WithTcpServer(_options.Host, _options.Port)
            .Build();

        await _mqttClient.ConnectAsync(options);
    }

    public async Task SubscribeAsync(string topic)
        => await _mqttClient.SubscribeAsync(new MqttTopicFilterBuilder()
            .WithTopic(topic)
            .Build());


    private async Task OnMessageReceived(MqttApplicationMessageReceivedEventArgs e)
    {
        var topic = e.ApplicationMessage.Topic;
        var payload = Encoding.UTF8.GetString(e.ApplicationMessage.Payload);

        _logger.LogInformation($"Received message on topic {topic}: {payload}");
        using var scope = _serviceProvider.CreateScope();
        
        var dbContext = scope.ServiceProvider.GetRequiredService<PressureLoggerContext>();
        var sender = scope.ServiceProvider.GetRequiredService<IMessageSender>();
        
        try
        {
            var messages = JsonSerializer.Deserialize<List<SendPressureLogRequestRange>>(payload);

            if (messages is not null)
            {
                List<PressureHistory> logs = new(capacity: messages.Count());

                foreach (var log in messages)
                {
                    logs.Add(PressureHistory.Create(log.Weight, log.CreatedAt));
                }

                await dbContext.PressureHistories.AddRangeAsync(logs);
                await dbContext.SaveChangesAsync();
                await sender.SendWeight(logs.Last().ValueInKilograms, logs.Last().CreatedAt);
            }
        }
        catch (Exception ex)
        {
            _logger.LogError("Failed to received message.");
            _logger.LogError($"Stack trance {ex.StackTrace}");
        }
    }
}