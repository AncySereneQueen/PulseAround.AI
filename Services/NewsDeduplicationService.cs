using PulseAround.AI.Models;

namespace PulseAround.AI.Services;

public class NewsDeduplicationService
{
    public List<NewsArticle> RemoveDuplicates(IEnumerable<NewsArticle> articles)
    {
        var urls = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
        var titles = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
        var unique = new List<NewsArticle>();

        foreach (var article in articles)
        {
            string url = article.SourceUrl.Trim();
            string title = article.Title.Trim();
            if (string.IsNullOrEmpty(title)) continue;
            if ((url.Length > 0 && urls.Contains(url)) || titles.Contains(title))
                continue;
            if (url.Length > 0) urls.Add(url);
            titles.Add(title);
            unique.Add(article);
        }
        return unique;
    }
}
