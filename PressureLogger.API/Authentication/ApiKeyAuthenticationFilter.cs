namespace PressureLogger.API.Authentication;

public class ApiKeyAuthenticationFilter : IEndpointFilter
{
	private const string ApiKeyHeader = "X-Api-Key";
	private readonly HashSet<string> _validApiKeys;

	public ApiKeyAuthenticationFilter(IConfiguration configuration)
	{
		_validApiKeys = [.. configuration.GetSection("ApiKey:Keys").Get<List<string>>()!];
	}

	public async ValueTask<object?> InvokeAsync(
		EndpointFilterInvocationContext context,
		EndpointFilterDelegate next)
	{
		if (!IsValid(context.HttpContext))
		{
			return Results.Unauthorized();
		}

		return await next(context);
	}

	private bool IsValid(HttpContext context)
	{
		string? apiKey = context.Request.Headers[ApiKeyHeader];

		if (string.IsNullOrEmpty(apiKey))
		{
			return false;
		}

		return _validApiKeys.Contains(apiKey);
	}
}
