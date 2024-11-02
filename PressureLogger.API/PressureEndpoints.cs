using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using PressureLogger.API.Authentication;
using PressureLogger.API.Hub;
using PressureLogger.API.Requests;
using PressureLogger.Infrastructure.DAL;

namespace PressureLogger.API;

public static class PressureEndpoints
{
	public static void MapPressureEndpoints(this WebApplication builder)
	{
		var group = builder
			.MapGroup("api/pressure")
			.WithTags("Pressure");

		group.MapPost("/", async (
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
		}).AddEndpointFilter<ApiKeyAuthenticationFilter>();

		group.MapGet("{id:guid}", async (PressureLoggerContext context, Guid id, CancellationToken ct) =>
		{
			var log = await context
				.PressureHistories
				.FirstOrDefaultAsync(x => x.Id == id, ct);

			return Results.Ok(log);
		});

		group.MapGet("{date:datetime}", async (DateTime date, PressureLoggerContext context, CancellationToken ct) =>
		{
			var histories = await context
				.PressureHistories
				.Where(x => x.CreatedAt.Date == date.Date)
				.OrderBy(x => x.CreatedAt)
				.ToListAsync(ct);

			return Results.Ok(histories);
		});

		group.MapGet("/last", async (PressureLoggerContext context, CancellationToken ct) =>
		{
			var result = await context
				.PressureHistories
				.OrderByDescending(x => x.CreatedAt)
				.FirstOrDefaultAsync(ct);
			return result is not null ? Results.Ok(result) : Results.NotFound();
		});

		group.MapGet("/", async (PressureLoggerContext context,
			[FromQuery] DateTime begin, [FromQuery] DateTime end, CancellationToken ct) =>
		{
			var histories = await context
				.PressureHistories
				.Where(x => x.CreatedAt >= begin && x.CreatedAt <= end)
				.OrderBy(x => x.CreatedAt)
				.ToListAsync(ct);

			return Results.Ok(histories);
		});
	}
}
