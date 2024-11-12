using System.Text.Json.Serialization;

namespace PressureLogger.Infrastructure.Models;

public sealed record SendPressureLogRequestRange(
    [property: JsonPropertyName("w")]double Weight,
    [property: JsonPropertyName("c")]DateTime CreatedAt);