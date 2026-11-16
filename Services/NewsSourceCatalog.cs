namespace PulseAround.AI.Services;

// Approved sources only. Add or replace sources here, not in Program.cs or the agent.
public static class NewsSourceCatalog
{
    public static readonly IReadOnlyDictionary<string, string> Feeds =
        new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase)
        {
            ["technology"] = "https://feeds.bbci.co.uk/news/technology/rss.xml",
            ["security"] = "https://www.bleepingcomputer.com/feed/",
            ["innovation"] = "https://www.sciencedaily.com/rss/top/technology.xml",
            ["ai"] = "https://techcrunch.com/category/artificial-intelligence/feed/",
            ["software"] = "https://devblogs.microsoft.com/dotnet/feed/",
            ["space"] = "https://www.nasa.gov/feed/",
            ["gadgets"] = "https://www.theverge.com/rss/index.xml",
            ["automotive"] = "https://electrek.co/feed/"
        };
}
