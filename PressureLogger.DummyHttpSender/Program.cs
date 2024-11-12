using Newtonsoft.Json;
using System.Text;

HttpClient client = new HttpClient();
client.DefaultRequestHeaders.Add("X-Api-Key", "qwerty");
Console.WriteLine("Za kilka sekund zaczynam wysyłać wiadomosći ...");
await Task.Delay(5000);

Console.WriteLine("Zaczynamy!");

while (true)
{
    await SendPressureData();
    await Task.Delay(6_000);
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

    var content = new StringContent(json, Encoding.UTF8, "application/json");

    try
    {
        var response = await client.PostAsync("https://localhost:6001/api/pressure/range", content);
        response.EnsureSuccessStatusCode();

        string responseBody = await response.Content.ReadAsStringAsync();
        Console.WriteLine($"Wysłano dane: Weight = {weight}. Odpowiedź: {responseBody}");
    }
    catch (HttpRequestException e)
    {
        Console.WriteLine($"Błąd podczas wysyłania zapytania: {e.Message}");
    }
}

class Request
{
    public double Weight { get; set; }
    public DateTime CreatedAt { get; set; }
}