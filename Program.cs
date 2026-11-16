using PulseAround.AI.Agents;
using PulseAround.AI.Helpers;
using PulseAround.AI.Services;

bool useAi = args.Contains("--ai", StringComparer.OrdinalIgnoreCase);
bool refresh = args.Contains("--refresh", StringComparer.OrdinalIgnoreCase);

ConsoleDisplay.ShowHeader(useAi ? "AI AGENT" : "DEVELOPMENT");

var result = await new NewsPipelineService().GetNewsAsync(refresh);
ConsoleDisplay.ShowCoverage(result);

var analysisCache = new NewsAnalysisCacheService();
await analysisCache.LoadAsync();

if (useAi)
{
    // A fully cached run requires no API key and makes no OpenAI calls.
    var candidates = NewsAgent.SelectCandidates(result.Articles);
    bool needsAi = candidates.Any(a => !analysisCache.TryGet(a, out _));
    if (needsAi)
    {
        string apiKey = Environment.GetEnvironmentVariable("OPENAI_PULSE_API_KEY")
            ?? throw new InvalidOperationException("OPENAI_PULSE_API_KEY is missing.");
        var agent = new NewsAgent(new OpenAIClientProvider(apiKey));
        await agent.RunAsync(candidates, analysisCache);
    }
    else
    {

        ConsoleDisplay.ShowAnalysisHeader();

        int shown = 0;

        foreach (var article in result.Articles)
        {
            if (!analysisCache.TryGet(article, out var analysis))
                continue;

            shown++;
            ConsoleDisplay.ShowAnalysis(article, analysis, shown);
        }

        ConsoleDisplay.ShowAnalysisCount(shown);

        Console.WriteLine("  All selected analyses loaded from cache (no API calls).");
    }
}
else
{
    // Show cached AI analyses where available; otherwise show original RSS details.
    ConsoleDisplay.ShowArticlesWithCachedAnalysis(result.Articles, analysisCache);
    ConsoleDisplay.ShowHint();
}
