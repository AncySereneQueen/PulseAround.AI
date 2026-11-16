using System.Text.Json;
using OpenAI.Responses;
using PulseAround.AI.Helpers;
using PulseAround.AI.Models;
using PulseAround.AI.Schemas;
using PulseAround.AI.Services;

namespace PulseAround.AI.Agents;

#pragma warning disable OPENAI001

// The model can request only pre-fetched, filtered news. No arbitrary URL or shell access.
public sealed class NewsAgent
{
    private readonly ResponsesClient _client;

    public NewsAgent(OpenAIClientProvider provider) => _client = provider.Client;


    public static List<NewsArticle> SelectCandidates(
        IReadOnlyList<NewsArticle> articles)
    {
        return articles.ToList();
    }


    public async Task RunAsync(
        IReadOnlyList<NewsArticle> candidates,
        NewsAnalysisCacheService cache)
    {
        var pending = candidates.Where(a => !cache.TryGet(a, out _)).ToList();
        if (candidates.Count == 0)
        {
            ConsoleDisplay.ShowWarning("No relevant articles available for AI analysis.");
            return;
        }
        if (pending.Count == 0)
        {
            ConsoleDisplay.ShowAnalysisHeader();

            int shown = 0;

            foreach (var article in candidates)
            {
                if (!cache.TryGet(article, out var analysis))
                    continue;

                shown++;
                ConsoleDisplay.ShowAnalysis(article, analysis, shown);
            }

            ConsoleDisplay.ShowAnalysisCount(shown);
            return;
        }

        var payload = pending.Select(a => new
        {
            a.Title, a.Category,
            Description = a.Description.Length > 250 ? a.Description[..250] : a.Description,
            a.SourceName, a.SourceUrl,
            PublishedAt = a.PublishedAt.ToString("yyyy-MM-dd")
        }).ToList();

        var tool = ResponseTool.CreateFunctionTool(
            functionName: "GetNewsCandidates",
            functionDescription: "Retrieve the already-fetched and filtered RSS news candidates.",
            functionParameters: BinaryData.FromString("""
                {"type":"object","properties":{},"required":[],"additionalProperties":false}
                """),
            strictModeEnabled: true);

        var input = new List<ResponseItem>
        {
            ResponseItem.CreateDeveloperMessageItem("""
                You are PulseAround.AI, a read-only technology news agent.
                Call GetNewsCandidates before producing the news analysis.
                Prioritize actionable security, AI, developer releases, innovation,
                space, gadgets and automotive developments. Exclude personal gossip.
                Analyze every supplied news candidate.
                Return exactly one analysis for each candidate.
                Do not omit any candidate.
                Use only facts in the supplied candidates; do not invent details.
                For each story, use its EXACT sourceUrl from the candidates.
                Explain why it matters, and assign High, Medium or Low importance.
                Verification is NOT performed: always use NotVerified.
                Include up to three specific claimsToVerify, or an empty array.
                """),
            ResponseItem.CreateUserMessageItem(
                "Prepare a structured technology news analysis from the available RSS sources.")
        };

        bool toolWasCalled = false;
        for (int step = 0; step < 3; step++)
        {
            var options = new CreateResponseOptions { Model = OpenAIClientProvider.Model };
            if (!toolWasCalled)
            {
                options.Tools.Add(tool);
            }
            else
            {
                options.TextOptions = new ResponseTextOptions
                {
                    TextFormat = ResponseTextFormat.CreateJsonSchemaFormat(
                        jsonSchemaFormatName: "news_digest",
                        jsonSchema: BinaryData.FromString(NewsDigestSchema.Json),
                        jsonSchemaIsStrict: true)
                };
            }

            foreach (ResponseItem item in input) options.InputItems.Add(item);
            ResponseResult response = await _client.CreateResponseAsync(options);
            input.AddRange(response.OutputItems);

            var calls = response.OutputItems.OfType<FunctionCallResponseItem>().ToList();
            if (calls.Count > 0)
            {
                foreach (var call in calls)
                {
                    string output = call.FunctionName == "GetNewsCandidates"
                        ? JsonSerializer.Serialize(payload)
                        : JsonSerializer.Serialize(new { error = "Unknown tool" });
                    input.Add(new FunctionCallOutputResponseItem(call.CallId, output));
                    if (call.FunctionName == "GetNewsCandidates") toolWasCalled = true;
                }
                continue;
            }

            if (!toolWasCalled)
            {
                ConsoleDisplay.ShowWarning("Agent did not request its news tool; no digest produced.");
                return;
            }

            string json = response.GetOutputText();
            if (string.IsNullOrWhiteSpace(json))
            {
                ConsoleDisplay.ShowWarning("Agent returned no structured analysis.");
                return;
            }

            NewsDigest digest = JsonSerializer.Deserialize<NewsDigest>(json)
                ?? throw new InvalidDataException("Empty AI news digest.");

            // Never display model-invented links. Match each item to a fetched article.
            var byUrl = pending.GroupBy(a => a.SourceUrl, StringComparer.OrdinalIgnoreCase)
                .ToDictionary(g => g.Key, g => g.First(), StringComparer.OrdinalIgnoreCase);
            var displayed = new HashSet<string>(StringComparer.OrdinalIgnoreCase);

            foreach (NewsDigestEntry entry in digest.Items)
            {
                if (!byUrl.TryGetValue(entry.SourceUrl, out NewsArticle? article)
                    || !displayed.Add(entry.SourceUrl)) continue;
                entry.Analysis.VerificationStatus = "NotVerified";
                cache.Set(article, entry.Analysis);
            }
            if (displayed.Count == 0)
                ConsoleDisplay.ShowWarning("No AI results matched known article sources.");
            else
                await cache.SaveAsync();

            ConsoleDisplay.ShowAnalysisHeader();
            int shown = 0;
            foreach (var candidate in candidates)
            {
                if (!cache.TryGet(candidate, out var analysis)) continue;

                shown++;
                ConsoleDisplay.ShowAnalysis(candidate, analysis, shown);

            }
            ConsoleDisplay.ShowAnalysisCount(shown);
            if (pending.Count > displayed.Count)
                ConsoleDisplay.ShowWarning($"{pending.Count - displayed.Count} uncached candidates were not analyzed; they can be processed in a later run.");
            return;
        }
        ConsoleDisplay.ShowWarning("Agent reached its step limit.");
    }
}
