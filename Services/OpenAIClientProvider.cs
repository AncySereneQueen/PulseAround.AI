using OpenAI.Responses;

namespace PulseAround.AI.Services;

#pragma warning disable OPENAI001

public class OpenAIClientProvider
{
    public ResponsesClient Client { get; }
    public const string Model = "gpt-6-astra";

    public OpenAIClientProvider(string apiKey)
    {
        Client = new ResponsesClient(apiKey);
    }
}
