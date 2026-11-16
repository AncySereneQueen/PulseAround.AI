using PulseAround.AI.Models;
using PulseAround.AI.Services;

namespace PulseAround.AI.Helpers;

public static class ConsoleDisplay
{
    public static void ShowHeader(string mode)
    {
        WriteLine(new string('═', 60), ConsoleColor.Cyan);
        WriteLine("                    PULSE-AROUND.AI", ConsoleColor.Cyan);
        WriteLine(new string('═', 60), ConsoleColor.Cyan);
        WriteLine($"  GLOBAL TECHNOLOGY INTELLIGENCE | {mode}", ConsoleColor.DarkCyan);
        Console.WriteLine();
    }

    public static void ShowCoverage(NewsRunResult result)
    {
        Section(result.FromCache ? "CACHED NEWS" : "LIVE RSS COLLECTION");
        foreach (string category in NewsSourceCatalog.Feeds.Keys)
        {
            int count = result.CategoryCounts.GetValueOrDefault(category);
            bool failed = result.Errors.TryGetValue(category, out string? error);
            WriteLine($"  {category,-14} {count,2} articles",
                failed ? ConsoleColor.Red : ConsoleColor.Green);
            if (failed) WriteLine($"    Error: {error}", ConsoleColor.Red);
        }
        WriteLine($"\n  Raw: {result.RawCount} | Relevant unique: {result.Articles.Count}",
            ConsoleColor.Yellow);
    }

    public static void ShowArticles(IEnumerable<NewsArticle> articles)
    {
        Section("RELEVANT NEWS — LOCAL PREVIEW");
        int index = 0;
        foreach (NewsArticle article in articles)
        {
            WriteLine($"\n  {++index:00}  {article.Title}", ConsoleColor.White);
            Field("Category", article.Category, ConsoleColor.Cyan);
            Field("Source", article.SourceName, ConsoleColor.Gray);
            if (article.PublishedAt != default)
                Field("Published", article.PublishedAt.ToString("dd MMM yyyy, HH:mm"), ConsoleColor.Gray);
            Field("Description", article.Description, ConsoleColor.Gray);
            Field("Read More", article.SourceUrl, ConsoleColor.Blue);
            WriteLine("  " + new string('─', 56), ConsoleColor.DarkGray);
        }
        if (index == 0) ShowWarning("No articles passed the relevance filter.");
        else WriteLine($"\n  Displayed {index} relevant articles.", ConsoleColor.Green);
    }


    public static void ShowAnalysis(
        NewsArticle article,
        NewsAnalysis analysis,
        int number)
    {
        Console.WriteLine();
        Console.ForegroundColor = ConsoleColor.DarkGray;
        Console.WriteLine(new string('─', 65));

        Console.ForegroundColor = ConsoleColor.Cyan;
        Console.WriteLine($"{number:D2}. {analysis.Headline}");

        Console.ForegroundColor = ConsoleColor.Yellow;
        Console.WriteLine(
            $"    {analysis.Category} | {analysis.Importance} Importance");

        Console.ResetColor();

        Console.WriteLine($"    Summary: {analysis.Summary}");
        Console.WriteLine($"    Why it matters: {analysis.WhyItMatters}");

        Console.ForegroundColor = ConsoleColor.DarkYellow;
        Console.WriteLine(
            $"    Verification: {analysis.VerificationStatus}");

        Console.ResetColor();

        if (analysis.ClaimsToVerify?.Count > 0)
        {
            Console.WriteLine("    Claims to verify:");

            foreach (var claim in analysis.ClaimsToVerify)
                Console.WriteLine($"      - {claim}");
        }

        Console.ForegroundColor = ConsoleColor.DarkGray;
        Console.WriteLine($"    Source: {article.SourceName}");

        Console.ForegroundColor = ConsoleColor.Blue;
        Console.WriteLine($"    URL: {article.SourceUrl}");

        Console.ResetColor();
    }


    public static void ShowDigest(string digest)
    {
        Section("AI NEWS DIGEST — FEED CONTENT NOT INDEPENDENTLY VERIFIED");
        Console.WriteLine(digest);
    }

    public static void ShowWarning(string message) =>
        WriteLine($"  WARNING: {message}", ConsoleColor.Red);

    public static void ShowHint() =>
        WriteLine("\n  Commands: dotnet run | dotnet run -- --refresh | dotnet run -- --ai",
            ConsoleColor.DarkGray);

    private static void Section(string title) =>
        WriteLine($"\n● {title}", ConsoleColor.Yellow);

    private static void Field(string label, string? value, ConsoleColor color)
    {
        if (string.IsNullOrWhiteSpace(value)) return;
        Console.ForegroundColor = ConsoleColor.DarkYellow;
        Console.Write($"  {label,-16}: ");
        Console.ResetColor();
        WriteLine(value, color);
    }

    private static void WriteLine(string value, ConsoleColor color)
    {
        Console.ForegroundColor = color;
        Console.WriteLine(value);
        Console.ResetColor();
    }


    public static void ShowAnalysisHeader()
    {
        Console.WriteLine();

        Console.ForegroundColor = ConsoleColor.Cyan;
        Console.WriteLine(new string('═', 65));
        Console.WriteLine("              PULSEAROUND.AI — AI NEWS ANALYSIS");
        Console.WriteLine(new string('═', 65));
        Console.ResetColor();

        Console.WriteLine();
    }

    public static void ShowAnalysisCount(int count)
    {
        Console.WriteLine();

        Console.ForegroundColor = ConsoleColor.Green;
        Console.WriteLine(new string('═', 65));
        Console.WriteLine($"  Total AI-analyzed articles displayed: {count}");
        Console.WriteLine(new string('═', 65));
        Console.ResetColor();
    }


    public static void ShowArticlesWithCachedAnalysis(
        IEnumerable<NewsArticle> articles,
        NewsAnalysisCacheService cache)
    {
        ShowAnalysisHeader();

        int number = 0;
        int analyzedCount = 0;

        foreach (var article in articles)
        {
            number++;

            if (cache.TryGet(article, out var analysis))
            {
                ShowAnalysis(article, analysis, number);
                analyzedCount++;
            }
            else
            {
                Console.WriteLine();
                Console.ForegroundColor = ConsoleColor.DarkGray;
                Console.WriteLine(new string('-', 65));

                Console.ForegroundColor = ConsoleColor.Cyan;
                Console.WriteLine($"{number:D2}. {article.Title}");

                Console.ForegroundColor = ConsoleColor.Yellow;
                Console.WriteLine($"    Category: {article.Category}");

                Console.ResetColor();
                Console.WriteLine($"    Source: {article.SourceName}");

                Console.ForegroundColor = ConsoleColor.Blue;
                Console.WriteLine($"    URL: {article.SourceUrl}");

                Console.ForegroundColor = ConsoleColor.DarkYellow;
                Console.WriteLine("    AI Analysis: Not yet available");

                Console.ResetColor();
            }
        }

        Console.WriteLine();
        Console.ForegroundColor = ConsoleColor.Green;
        Console.WriteLine($"  Total articles displayed: {number}");
        Console.WriteLine($"  AI-analyzed: {analyzedCount}");
        Console.WriteLine($"  Awaiting AI analysis: {number - analyzedCount}");
        Console.ResetColor();
    }


}
