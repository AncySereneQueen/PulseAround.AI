using PulseAround.AI.Models;
using PulseAround.AI.Tools;

namespace PulseAround.AI.Services;

// Shared by both modes: one fetch/cache/filter path.
public class NewsPipelineService
{
    private readonly NewsCacheService _cache = new();
    private readonly NewsTools _tools = new();
    private readonly NewsProcessingService _processing = new();

    public async Task<NewsRunResult> GetNewsAsync(bool refresh)
    {
        List<NewsArticle>? cached = refresh ? null : await _cache.LoadAsync();
        bool fromCache = cached is { Count: > 0 };
        var raw = fromCache ? cached! : new List<NewsArticle>();
        var errors = new Dictionary<string, string>();
        var counts = new Dictionary<string, int>();

        if (!fromCache)
        {
            foreach (string category in NewsSourceCatalog.Feeds.Keys)
            {
                try
                {
                    var items = await _tools.GetLatestNewsAsync(category);
                    raw.AddRange(items);
                    counts[category] = items.Count;
                }
                catch (Exception ex)
                {
                    errors[category] = ex.Message;
                    counts[category] = 0;
                }
            }
            // Preserve successful articles, including partial retrievals.
            await _cache.SaveAsync(raw);
        }
        else
        {
            // Older cache files may not contain Category; refresh to restore it.
            foreach (string category in NewsSourceCatalog.Feeds.Keys)
                counts[category] = raw.Count(a =>
                    string.Equals(a.Category, category,
                        StringComparison.OrdinalIgnoreCase));
        }

        return new NewsRunResult
        {
            Articles = _processing.Process(raw),
            RawCount = raw.Count,
            CategoryCounts = counts,
            Errors = errors,
            FromCache = fromCache
        };
    }
}
