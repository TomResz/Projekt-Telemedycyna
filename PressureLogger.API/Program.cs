using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using PressureLogger.API.Hub;
using PressureLogger.API.Requests;
using PressureLogger.Infrastructure;
using PressureLogger.Infrastructure.DAL;
using static System.Runtime.InteropServices.JavaScript.JSType;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddSignalR();


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

app.UseHttpsRedirection();

app.MapHub<WeightHub>("/weighthub");

app.MapPost("api/pressure/", async (
	PressureLoggerContext context,
	SendPressureLogRequest request,
	IHubContext<WeightHub, IWeightClient> hubContext,
	CancellationToken ct) =>
{
	var log = PressureHistory.Create(request.Weight, DateTime.Now);

	await context.AddAsync(log, ct);

	await context.SaveChangesAsync(ct);

	await hubContext.Clients.All.SendWeight(log.ValueInKilograms, log.CreatedAt);

	return Results.Created($"api/pressure/{log.Id}", log);
});

app.MapGet("api/pressure/{id:guid}", async (PressureLoggerContext context, Guid id, CancellationToken ct) =>
{
	var log = await context
	.PressureHistories
	.FirstOrDefaultAsync(x => x.Id == id, ct);

	return Results.Ok(log);
});

app.MapGet("api/pressure/{date:datetime}", async (DateTime date, PressureLoggerContext context, CancellationToken ct) =>
{
	var histories = await context
	.PressureHistories
	.Where(x => x.CreatedAt.Date == date.Date)
	.OrderBy(x => x.CreatedAt)
	.ToListAsync(ct);
	return Results.Ok(histories);
});

app.MapGet("api/pressure/last", async (PressureLoggerContext context, CancellationToken ct) =>
{
	var result = await context
		.PressureHistories
		.OrderByDescending(x => x.CreatedAt)
		.FirstOrDefaultAsync(ct);
	return result is not null ? Results.Ok(result) : Results.NotFound();
});

app.MapGet("api/pressure/", async (PressureLoggerContext context,
	[FromQuery] DateTime begin, [FromQuery]DateTime end,CancellationToken ct) =>
{
	var histories = await context
	.PressureHistories
	.Where(x => x.CreatedAt >= begin && x.CreatedAt <= end)
	.OrderBy(x => x.CreatedAt)
	.ToListAsync(ct);

	return Results.Ok(histories);
});

app.Run();
