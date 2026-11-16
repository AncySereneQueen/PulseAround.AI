namespace PulseAround.AI.Models;

public class NewsArticle
{
    public string Title { get; set; } = "";
    public string Description { get; set; } = "";
    public string SourceUrl { get; set; } = "";
    public string SourceName { get; set; } = "";
    public string Category { get; set; } = "";
    public DateTimeOffset PublishedAt { get; set; }
}
