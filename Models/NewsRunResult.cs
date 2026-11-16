namespace PulseAround.AI.Models;

public class NewsRunResult
{
    public List<NewsArticle> Articles { get; init; } = new();
    public Dictionary<string, int> CategoryCounts { get; init; } = new();
    public Dictionary<string, string> Errors { get; init; } = new();
    public bool FromCache { get; init; }
    public int RawCount { get; init; }
}
