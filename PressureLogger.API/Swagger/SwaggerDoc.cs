using Microsoft.OpenApi.Models;

namespace PressureLogger.API.Swagger;

public static class SwaggerDoc
{
	public static IServiceCollection AddCustomSwaggerGen(this IServiceCollection services)
	{
		services.AddSwaggerGen(options =>
		{
			options.AddSecurityDefinition("ApiKey", new OpenApiSecurityScheme
			{
				Description = "API Key needed to access the endpoints. Choose from the list.",
				Name = "X-Api-Key",
				In = ParameterLocation.Header,
				Type = SecuritySchemeType.ApiKey,
				Scheme = "ApiKeyScheme"
			});

			options.AddSecurityRequirement(new OpenApiSecurityRequirement
			{
				{
					new OpenApiSecurityScheme
					{
						Reference = new OpenApiReference
						{
							Type = ReferenceType.SecurityScheme,
							Id = "ApiKey"
						},
						Scheme = "ApiKeyScheme",
						Name = "X-Api-Key",
						In = ParameterLocation.Header
					},
					new List<string>()
				}
			});

			options.OperationFilter<ApiKeyOperationFilter>();
		});
		return services;
	}
}
