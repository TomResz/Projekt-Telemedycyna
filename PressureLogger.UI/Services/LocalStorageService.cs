using Microsoft.JSInterop;
using System.Text.Json;

namespace PressureLogger.UI.Services;

public class LocalStorageService
{
	private readonly IJSRuntime _jsRuntime;

	public LocalStorageService(IJSRuntime jsRuntime)
	{
		_jsRuntime = jsRuntime;
	}

	public async Task<string?> GetValueAsync(string key)
	{
		return await _jsRuntime.InvokeAsync<string?>("localStorage.getItem", key);
	}

	public async Task<T?> GetValueAsync<T>(string key)
	{
		var value = await GetValueAsync(key);

		if (value is null)
		{
			return default;
		}
		return JsonSerializer.Deserialize<T>(value);
	}

	public async Task SetValueAsync(string key, string? value)
	{
		await _jsRuntime.InvokeAsync<string?>("localStorage.setItem", key, value);
	}

	public async Task SetValueAsync<T>(string key, T? value)
	{
		var json = JsonSerializer.Serialize(value);
		await SetValueAsync(key, json);
	}

	public async Task RemoveKeyAsync(string key)
	{
		await _jsRuntime.InvokeVoidAsync("localStorage.removeItem", key);
	}
}
