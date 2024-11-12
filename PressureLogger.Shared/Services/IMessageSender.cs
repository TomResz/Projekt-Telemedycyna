namespace PressureLogger.Shared.Services;

public interface IMessageSender
{
    Task SendWeight(double weight,DateTime createdAt);
}