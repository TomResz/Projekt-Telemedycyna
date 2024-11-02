using Microsoft.OpenApi.Any;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace PressureLogger.API.Swagger;

public class ApiKeyOperationFilter : IOperationFilter
{
	private readonly IConfiguration _configuration;

	public ApiKeyOperationFilter(IConfiguration configuration)
	{
		_configuration = configuration;
	}

	public void Apply(OpenApiOperation operation, OperationFilterContext context)
	{
		var apiKeys = _configuration.GetSection("ApiKey:Keys").Get<List<string>>() ?? [];

		if (operation.Parameters is null)
		{
			operation.Parameters = [];
		}

		operation.Parameters.Add(new OpenApiParameter
		{
			Name = "X-Api-Key",
			In = ParameterLocation.Header,
			Description = "Select API Key",
			Required = true,
			Schema = new OpenApiSchema
			{
				Type = "string",
				Enum = apiKeys
				.Select(k => new OpenApiString(k))
				.Cast<IOpenApiAny>()
				.ToList()
			}
		});
	}
}
