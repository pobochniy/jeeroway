using System.Text.Json.Serialization;

namespace Atheneum.Dto.Robo;

public record VideoStreamConfigDto(
    [property: JsonPropertyName("udpAddress")] string? UdpAddress = null,
    [property: JsonPropertyName("framerate")] int? Framerate = null,
    [property: JsonPropertyName("videoSize")] string? VideoSize = null
);
