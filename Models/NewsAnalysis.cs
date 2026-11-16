using System.Text.Json.Serialization;

namespace PulseAround.AI.Models;

public class NewsAnalysis
{
    [JsonPropertyName("headline")]
    public string Headline { get; set; } = "";
    [JsonPropertyName("summary")]
    public string Summary { get; set; } = "";
    [JsonPropertyName("category")]
    public string Category { get; set; } = "";
    [JsonPropertyName("importance")]
    public string Importance { get; set; } = "";
    [JsonPropertyName("whyItMatters")]
    public string WhyItMatters { get; set; } = "";
    [JsonPropertyName("verificationStatus")]
    public string VerificationStatus { get; set; } = "NotVerified";
    [JsonPropertyName("claimsToVerify")]
    public List<string> ClaimsToVerify { get; set; } = new();
}
