using Microsoft.AspNetCore.SignalR;

namespace PressureLogger.API.Hub;

public class WeightHub : Hub<IWeightClient>
{
}
public interface IWeightClient
{
	Task SendWeight(double weight,DateTime dateTime);
}