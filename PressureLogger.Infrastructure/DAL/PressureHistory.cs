namespace PressureLogger.Infrastructure.DAL;

public class PressureHistory
{
	public Guid Id { get; set; }
	public DateTime CreatedAt { get; set; }
    public double ValueInKilograms { get; set; }	

	public static PressureHistory Create(double valueInKilograms,DateTime currentDate)
	{
		return new PressureHistory
		{
			Id = Guid.NewGuid(),
			CreatedAt = currentDate,
			ValueInKilograms = valueInKilograms,
		};
	}
}