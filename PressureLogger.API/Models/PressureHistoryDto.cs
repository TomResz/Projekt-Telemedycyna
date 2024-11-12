namespace PressureLogger.API.Models;

public class PressureHistoryDto
{
    public DateTime CreatedAt { get; set; }
    public double ValueInKilograms { get; set; }
}