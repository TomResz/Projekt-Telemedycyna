using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using PressureLogger.API.Hub;
using PressureLogger.API.Requests;
using PressureLogger.Infrastructure;
using PressureLogger.Infrastructure.DAL;

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
	IHubContext<WeightHub,IWeightClient> hubContext,
	CancellationToken ct) =>
{
	var log = PressureHistory.Create(request.Weight, DateTime.Now);

	await context.AddAsync(log, ct);

	await context.SaveChangesAsync(ct);

	await hubContext.Clients.All.SendWeight(log.ValueInKilograms,log.CreatedAt);

	return Results.Created($"api/pressure/{log.Id}", log);
});

app.MapGet("api/pressure/{id:guid}", async (PressureLoggerContext context, Guid id,CancellationToken ct) =>
{
	var log = await context
	.PressureHistories
	.FirstOrDefaultAsync(x => x.Id == id,ct);
		
	return Results.Ok(log);
});

app.MapGet("api/pressure/{date:datetime}", async (DateTime date, PressureLoggerContext context,CancellationToken ct) =>
{
	var histories = await context
	.PressureHistories
	.Where(x=>x.CreatedAt.Date == date.Date)
	.OrderBy(x=>x.CreatedAt)
	.ToListAsync(ct);
	return Results.Ok(histories);
});

app.Run();
