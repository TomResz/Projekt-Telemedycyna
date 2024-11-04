namespace PressureLogger.API.Requests;

public sealed record SendPressureLogRequest(double Weight);

public sealed record SendPressureLogRequestRange(double Weight,DateTime CreatedAt);
