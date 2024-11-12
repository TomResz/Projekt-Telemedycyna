using Microsoft.AspNetCore.SignalR;
using PressureLogger.Shared.Services;

namespace PressureLogger.API.Hubs;

public sealed class MessageSender(IHubContext<WeightHub, IWeightClient> hubContext) : IMessageSender
{
    public async Task SendWeight(double weight, DateTime createdAt)
        => await hubContext.Clients.All.SendWeight(weight, createdAt);
}