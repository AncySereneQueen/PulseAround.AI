using System.Net;
using System.ServiceModel.Syndication;
using System.Text.RegularExpressions;
using System.Xml;
using PulseAround.AI.Models;

namespace PulseAround.AI.Services;

public class NewsFeedService
{
    private static readonly HttpClient Http = new()
    {
        Timeout = TimeSpan.FromSeconds(15)
    };
    private const int MaxFeedBytes = 2_000_000;

    public async Task<List<NewsArticle>> GetLatestNewsAsync(
        string category, string feedUrl)
    {
        using var response = await Http.GetAsync(
            feedUrl, HttpCompletionOption.ResponseHeadersRead);
        response.EnsureSuccessStatusCode();

        if (response.Content.Headers.ContentLength > MaxFeedBytes)
            throw new InvalidDataException("RSS response is too large.");

        await using var network = await response.Content.ReadAsStreamAsync();
        using var buffer = new MemoryStream();
        byte[] bytes = new byte[8192];
        int count;
        while ((count = await network.ReadAsync(bytes)) > 0)
        {
            if (buffer.Length + count > MaxFeedBytes)
                throw new InvalidDataException("RSS response exceeds size limit.");
            buffer.Write(bytes, 0, count);
        }
        buffer.Position = 0;

        var settings = new XmlReaderSettings
        {
            DtdProcessing = DtdProcessing.Prohibit,
            XmlResolver = null,
            MaxCharactersInDocument = MaxFeedBytes
        };
        using var reader = XmlReader.Create(buffer, settings);
        var feed = SyndicationFeed.Load(reader)
            ?? throw new InvalidDataException("Invalid RSS feed.");

        return feed.Items.Take(5).Select(item => new NewsArticle
        {
            Title = Clean(item.Title?.Text),
            Description = Clean(item.Summary?.Text),
            SourceUrl = item.Links.FirstOrDefault(
                link => link.RelationshipType == "alternate")?.Uri.ToString() ?? "",
            SourceName = feed.Title?.Text ?? category,
            Category = category,
            PublishedAt = item.PublishDate
        }).Where(article => article.Title.Length > 0).ToList();
    }

    private static string Clean(string? text)
    {
        string plain = Regex.Replace(text ?? "", "<[^>]*>", " ");
        plain = WebUtility.HtmlDecode(plain);
        plain = Regex.Replace(plain, @"\s+", " ").Trim();
        return plain.Length <= 500 ? plain : plain[..500];
    }
}
