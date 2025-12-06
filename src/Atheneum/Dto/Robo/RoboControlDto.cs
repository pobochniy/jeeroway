using System;
using System.Text.Json.Serialization;

namespace Atheneum.Dto.Robo
{
    public class RoboControlDto
    {
        [JsonPropertyName("roboId")]
        public Guid RoboId { get; set; }

        [JsonPropertyName("timeJs")]
        public long TimeJs { get; set; }

        [JsonPropertyName("w")]
        public bool W { get; set; }

        [JsonPropertyName("s")]
        public bool S { get; set; }

        [JsonPropertyName("a")]
        public bool A { get; set; }

        [JsonPropertyName("d")]
        public bool D { get; set; }
    }
}
