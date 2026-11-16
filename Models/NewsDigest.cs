using System.Text.Json.Serialization;

namespace PulseAround.AI.Models;

public sealed class NewsDigest
{
    [JsonPropertyName("items")]
    public List<NewsDigestEntry> Items { get; set; } = new();
}

public sealed class NewsDigestEntry
{
    [JsonPropertyName("sourceUrl")]
    public string SourceUrl { get; set; } = "";

    [JsonPropertyName("analysis")]
    public NewsAnalysis Analysis { get; set; } = new();
}
