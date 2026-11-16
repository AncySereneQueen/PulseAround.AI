using System.Text.Json;
using PulseAround.AI.Models;

namespace PulseAround.AI.Services;

public class NewsCacheService
{
    private readonly string _path = Path.Combine(
        AppContext.BaseDirectory, "Cache", "news-cache.json");

    public async Task<List<NewsArticle>?> LoadAsync()
    {
        if (!File.Exists(_path)) return null;
        string json = await File.ReadAllTextAsync(_path);
        return JsonSerializer.Deserialize<List<NewsArticle>>(json);
    }

    public async Task SaveAsync(IEnumerable<NewsArticle> articles)
    {
        Directory.CreateDirectory(Path.GetDirectoryName(_path)!);
        string json = JsonSerializer.Serialize(articles,
            new JsonSerializerOptions { WriteIndented = true });
        await File.WriteAllTextAsync(_path, json);
    }
}
