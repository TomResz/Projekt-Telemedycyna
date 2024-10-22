using Microsoft.EntityFrameworkCore;

namespace PressureLogger.Infrastructure.DAL;
public sealed class PressureLoggerContext : DbContext
{
    public DbSet<PressureHistory> PressureHistories { get; set; }

    public PressureLoggerContext(DbContextOptions<PressureLoggerContext> options) : base(options)	
    {
    }

	protected override void OnModelCreating(ModelBuilder modelBuilder)
	{
		modelBuilder.ApplyConfigurationsFromAssembly(typeof(PressureLoggerContext).Assembly);
	}
}
