using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PressureLogger.API.Models;
using PressureLogger.Infrastructure.DAL;

namespace PressureLogger.API;

public static class PressureEndpoints
{
	public static void MapPressureEndpoints(this WebApplication builder)
	{
		var group = builder
			.MapGroup("api/pressure")
			.WithTags("Pressure");
		

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

		group.MapGet("/avg-sec", async (PressureLoggerContext ctx,
			[FromQuery] DateTime begin, [FromQuery] DateTime end, CancellationToken ct) =>
		{
			var result = await ctx.Database
				.SqlQuery<PressureHistoryDto>($@"
        SELECT 
            strftime('%Y-%m-%d %H:%M:%S', CreatedAt) AS CreatedAt,
            AVG(ValueInKilograms) AS ValueInKilograms
        FROM 
            PressureHistories
        WHERE 
            CreatedAt BETWEEN {begin} AND {end}
        GROUP BY 
            strftime('%Y-%m-%d %H:%M:%S', CreatedAt)
        ORDER BY 
            CreatedAt")
				.ToListAsync(ct);
			
			return Results.Ok(result);	
		});
		
		group.MapGet("/avg-min", async (PressureLoggerContext ctx,
			[FromQuery] DateTime begin, [FromQuery] DateTime end, CancellationToken ct) =>
		{
			var result = await ctx.Database
				.SqlQuery<PressureHistoryDto>($@"
        SELECT 
            strftime('%Y-%m-%d %H:%M', CreatedAt) AS CreatedAt,
            AVG(ValueInKilograms) AS ValueInKilograms
        FROM 
            PressureHistories
        WHERE 
            CreatedAt BETWEEN {begin} AND {end}
        GROUP BY 
            strftime('%Y-%m-%d %H:%M', CreatedAt)
        ORDER BY 
            CreatedAt")
				.ToListAsync(ct);
			
			return Results.Ok(result);	
		});
	}
	
	
	
}