namespace PulseAround.AI.Models;

public sealed class CachedNewsAnalysis
{
    public string SourceUrl { get; set; } = "";
    public string ArticleFingerprint { get; set; } = "";
    public NewsAnalysis Analysis { get; set; } = new();
}
