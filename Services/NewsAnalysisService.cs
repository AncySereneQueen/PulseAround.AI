using System.Text.Json;
using OpenAI.Responses;
using PulseAround.AI.Models;
using PulseAround.AI.Schemas;

namespace PulseAround.AI.Services;

#pragma warning disable OPENAI001

// Optional per-article analysis, retained for later use. The agent digest
// currently uses NewsAgent, so it does not call this service on every article.
public class NewsAnalysisService
{
    private readonly ResponsesClient _client;

    public NewsAnalysisService(OpenAIClientProvider provider)
    {
        _client = provider.Client;
    }

    public async Task<NewsAnalysis> AnalyzeAsync(string article)
    {
        var options = new CreateResponseOptions
        {
            Model = OpenAIClientProvider.Model,
            TextOptions = new ResponseTextOptions
            {
                TextFormat = ResponseTextFormat.CreateJsonSchemaFormat(
                    jsonSchemaFormatName: "news_analysis",
                    jsonSchema: BinaryData.FromString(NewsAnalysisSchema.Json),
                    jsonSchemaIsStrict: true)
            }
        };
        options.InputItems.Add(ResponseItem.CreateDeveloperMessageItem("""
            Summarize the supplied news without inventing facts.
            Extract up to three claims that need independent verification.
            Set verificationStatus to NotVerified. Avoid gossip.
            """));
        options.InputItems.Add(ResponseItem.CreateUserMessageItem(article));

        ResponseResult response = await _client.CreateResponseAsync(options);
        return JsonSerializer.Deserialize<NewsAnalysis>(response.GetOutputText())
            ?? throw new InvalidDataException("Empty analysis result.");
    }
}
