using Microsoft.Extensions.DependencyInjection;

namespace PressureLogger.Infrastructure.DAL;
public static class DatabaseInitializer
{
	public static void EnsureDatabaseCreated(IServiceProvider serviceProvider)
	{
		var path = Path.Combine(Directory.GetCurrentDirectory(), "database");
		Console.WriteLine(path);
		if (!Directory.Exists(path))
		{
			Directory.CreateDirectory(path);
		}

		using var scope = serviceProvider.CreateScope();
		var dbContext = scope.ServiceProvider.GetRequiredService<PressureLoggerContext>();
		dbContext.Database.EnsureCreated();
	}

}
