using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace PressureLogger.Infrastructure.DAL;
internal sealed class PressureHistoryEntityConfiguration : IEntityTypeConfiguration<PressureHistory>
{
	public void Configure(EntityTypeBuilder<PressureHistory> builder)
	{
		builder.Property(p => p.Id)
	   .HasConversion(
		   guid => guid.ToByteArray(), 
		   bytes => new Guid(bytes))   
	   .HasColumnType("BLOB")        
	   .IsRequired();

		builder.Property(p => p.CreatedAt)
			   .IsRequired();

		builder.Property(p => p.ValueInKilograms)
			   .IsRequired()
			   .HasPrecision(10, 2);
	}
}
