using PulseAround.AI.Models;
using PulseAround.AI.Services;

namespace PulseAround.AI.Tools;

public class NewsTools
{
    private readonly NewsFeedService _feeds;

    public NewsTools(NewsFeedService? feeds = null)
    {
        _feeds = feeds ?? new NewsFeedService();
    }

    public Task<List<NewsArticle>> GetLatestNewsAsync(string category)
    {
        if (!NewsSourceCatalog.Feeds.TryGetValue(category, out string? url))
            throw new ArgumentException("Unsupported news category.", nameof(category));

        return _feeds.GetLatestNewsAsync(category, url);
    }
}
