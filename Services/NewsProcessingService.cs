using PulseAround.AI.Models;

namespace PulseAround.AI.Services;

public class NewsProcessingService
{
    private readonly NewsDeduplicationService _deduplication = new();
    private readonly NewsRelevanceService _relevance = new();

    public List<NewsArticle> Process(IEnumerable<NewsArticle> articles) =>
        _relevance.FilterRelevant(_deduplication.RemoveDuplicates(articles));
}
