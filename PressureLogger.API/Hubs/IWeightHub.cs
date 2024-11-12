namespace PressureLogger.API.Hubs;

public interface IWeightClient
{
	Task SendWeight(double weight,DateTime dateTime);
}