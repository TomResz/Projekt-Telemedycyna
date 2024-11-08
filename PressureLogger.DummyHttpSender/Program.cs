﻿using Newtonsoft.Json;
using System.Text;

HttpClient client = new HttpClient();
client.DefaultRequestHeaders.Add("X-Api-Key", "qwerty");
Console.WriteLine("Za kilka sekund zaczynam wysyłać wiadomosći ...");
await Task.Delay(5000);

Console.WriteLine("Zaczynamy!");

while (true)
{
	await SendPressureData();
	await Task.Delay(2_000);
}

async Task SendPressureData()
{
	var random = new Random();
	double weight = random.NextDouble(); 

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