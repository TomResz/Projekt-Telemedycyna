using Microsoft.Extensions.DependencyInjection;

namespace PressureLogger.Infrastructure.DAL;
public static class DatabaseInitializer
{
	public static void EnsureDatabaseCreated(IServiceProvider serviceProvider)
	{
		using (var scope = serviceProvider.CreateScope())
		{
			var dbContext = scope.ServiceProvider.GetRequiredService<PressureLoggerContext>();

			dbContext.Database.EnsureCreated();
		}
	}

}
