using System.Diagnostics;
using MQTTnet;
using Newtonsoft.Json;

var factory = new MqttClientFactory();
var mqttClient = factory.CreateMqttClient();

string connectionString = Environment.GetEnvironmentVariable("DOTNET_RUNNING_IN_CONTAINER") == "true"
    ? "pressurelogger.mqtt"
    : "localhost";
Console.WriteLine($"Connecting to {connectionString}");

var mqttOptions = new MqttClientOptionsBuilder()
    .WithClientId("MeasurementPublisherConsole")
    .WithTcpServer(connectionString, 1883)
    .Build();

await mqttClient.ConnectAsync(mqttOptions);

Console.Write("Connected with MQTT Broker");

while (true)
{
    await SendPressureData();
    await Task.Delay(5_000);
}


async Task SendPressureData()
{
    var random = new Random();
    double weight = random.NextDouble();

    var models = new List<Request>(capacity: 20);
    
    for (int i = 0; i < 20; i++)
    {
        var request = new Request()
        {
            Weight = random.NextDouble(),
            CreatedAt = DateTime.Now,
        };
        models.Add(request);
    }
    
    var json = JsonConvert.SerializeObject(models);
    
    var message = new MqttApplicationMessageBuilder()
        .WithTopic("sensors/measurements")
        .WithPayload(json)
        .WithRetainFlag(false)
        .Build();
    
    var stopwatch = Stopwatch.StartNew();
    var result = await mqttClient.PublishAsync(message);
    stopwatch.Stop();
    
    if (result.IsSuccess)
    {
        Console.WriteLine("\n\n\n--------------------------------------");
        Console.WriteLine("Published Successfully");
        Console.WriteLine($"Time Elapsed: {stopwatch.Elapsed}");
        Console.WriteLine("--------------------------------------");
    }
}

class Request
{
    public double Weight { get; set; }
    public DateTime CreatedAt { get; set; }
}