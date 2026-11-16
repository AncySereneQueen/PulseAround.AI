using PulseAround.AI.Models;

namespace PulseAround.AI.Services;

public class NewsRelevanceService
{
    // Require a relevant technical subject, not merely a technology company's name.
    private static readonly string[] TechnicalTerms =
    {
        "artificial intelligence", "ai model", "machine learning", "llm",
        "software", "cybersecurity", "vulnerability", "zero-day", "patch",
        "malware", "ransomware", "data breach", "outage", "cloud",
        "technology", "innovation", "research", "quantum", "nasa",
        "spacex", "satellite", "spacecraft", "smartphone", "gadget",
        "processor", "chip", "robotics", "electric vehicle", "ev battery",
        "electric car", "developer", ".net", "windows", "github", "copilot",
        "space", "solar", "battery", "scientist", "scientific", "datacenter",
        "data center", "infrastructure", "semiconductor", "security update"
    };

    private static readonly string[] ExcludedTopics =
    {
        "affair", "dating", "divorce", "romance", "scandal", "love life",
        "relationship rumor", "personal allegation", "celebrity gossip",
        "red carpet", "extramarital", "mistress"
    };

    public List<NewsArticle> FilterRelevant(IEnumerable<NewsArticle> articles) =>
        articles.Where(IsRelevant).ToList();

    private static bool IsRelevant(NewsArticle article)
    {
        string title = article.Title ?? "";
        string text = $"{title} {article.Description}";
        // Personal-life stories should not be rescued by incidental mentions of AI or Nvidia.
        if (ExcludedTopics.Any(term => text.Contains(term, StringComparison.OrdinalIgnoreCase)))
            return false;
        return TechnicalTerms.Any(term => text.Contains(term, StringComparison.OrdinalIgnoreCase));
    }
}
