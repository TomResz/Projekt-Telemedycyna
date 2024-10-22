using Newtonsoft.Json;
using System.Text;

HttpClient client = new HttpClient();
Console.WriteLine("Za kilka sekund zaczynam wysałać wiadomośći ...");
await Task.Delay(5000);

Console.WriteLine("Zaczynamy!");

while (true)
{
	await SendPressureData();
	await Task.Delay(5000);
}

async Task SendPressureData()
{
	var random = new Random();
	double weight = random.NextDouble() * 9 + 1; 

	var data = new { Weight = weight };
	var json = JsonConvert.SerializeObject(data);

	var content = new StringContent(json, Encoding.UTF8, "application/json");

	try
	{
		var response = await client.PostAsync("https://localhost:6001/api/pressure", content);
		response.EnsureSuccessStatusCode();

		string responseBody = await response.Content.ReadAsStringAsync();
		Console.WriteLine($"Wysłano dane: Weight = {weight}. Odpowiedź: {responseBody}");
	}
	catch (HttpRequestException e)
	{
		Console.WriteLine($"Błąd podczas wysyłania zapytania: {e.Message}");
	}
}