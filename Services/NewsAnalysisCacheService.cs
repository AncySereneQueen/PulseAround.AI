using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using PulseAround.AI.Models;

namespace PulseAround.AI.Services;

// Reuses AI analysis only while the underlying RSS article remains unchanged.
public sealed class NewsAnalysisCacheService
{
    private readonly string _path = Path.Combine(
        AppContext.BaseDirectory, "Cache", "analysis-cache.json");

    private readonly Dictionary<string, CachedNewsAnalysis> _entries =
        new(StringComparer.OrdinalIgnoreCase);

    public async Task LoadAsync()
    {
        _entries.Clear();
        if (!File.Exists(_path)) return;

        var json = await File.ReadAllTextAsync(_path);
        var entries = JsonSerializer.Deserialize<List<CachedNewsAnalysis>>(json) ?? new();
        foreach (var entry in entries)
        {
            if (!string.IsNullOrWhiteSpace(entry.SourceUrl))
                _entries[entry.SourceUrl] = entry;
        }
    }

    public bool TryGet(NewsArticle article, out NewsAnalysis analysis)
    {
        if (_entries.TryGetValue(article.SourceUrl, out var entry)
            && entry.ArticleFingerprint == Fingerprint(article))
        {
            analysis = entry.Analysis;
            analysis.VerificationStatus = "NotVerified";
            return true;
        }

        analysis = null!;
        return false;
    }

    public void Set(NewsArticle article, NewsAnalysis analysis)
    {
        analysis.VerificationStatus = "NotVerified";
        _entries[article.SourceUrl] = new CachedNewsAnalysis
        {
            SourceUrl = article.SourceUrl,
            ArticleFingerprint = Fingerprint(article),
            Analysis = analysis
        };
    }

    public async Task SaveAsync()
    {
        Directory.CreateDirectory(Path.GetDirectoryName(_path)!);
        var json = JsonSerializer.Serialize(_entries.Values,
            new JsonSerializerOptions { WriteIndented = true });
        await File.WriteAllTextAsync(_path, json);
    }

    private static string Fingerprint(NewsArticle article)
    {
        var content = string.Join("\n", article.Title, article.Description,
            article.Category, article.SourceUrl);
        return Convert.ToHexString(SHA256.HashData(Encoding.UTF8.GetBytes(content)));
    }
}
