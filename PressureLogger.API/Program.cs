using PressureLogger.API;
using PressureLogger.API.Hubs;
using PressureLogger.API.Swagger;
using PressureLogger.Infrastructure;
using PressureLogger.Infrastructure.DAL;
using PressureLogger.Infrastructure.MQTT;
using PressureLogger.Shared.Services;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddCustomSwaggerGen();

builder.Services.AddSignalR();
builder.Services.AddScoped<IMessageSender, MessageSender>();
builder.Services.AddInfrastructure(builder.Configuration);

builder.Services.AddCors(options =>
{
	options.AddPolicy("Frontend", builder =>
	{
		builder
			.AllowAnyMethod()
			.AllowAnyHeader()
			.AllowAnyOrigin()
			.WithExposedHeaders("*");
	});
});

var app = builder.Build();

app.UseCors("Frontend");

if (app.Environment.IsDevelopment())
{
	app.UseSwagger();
	app.UseSwaggerUI();
}

DatabaseInitializer.EnsureDatabaseCreated(app.Services);
await MqttClientInitializer.Start(app.Services);

app.UseHttpsRedirection();

app.MapHub<WeightHub>("/weighthub");

app.MapPressureEndpoints();

app.Run();
