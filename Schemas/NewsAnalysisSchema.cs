namespace PulseAround.AI.Schemas;

public static class NewsAnalysisSchema
{
    public const string Json = """
    {
      "type": "object",
      "properties": {
        "headline": { "type": "string" },
        "summary": { "type": "string" },
        "category": { "type": "string" },
        "importance": { "type": "string", "enum": ["High", "Medium", "Low"] },
        "whyItMatters": { "type": "string" },
        "verificationStatus": { "type": "string", "enum": ["NotVerified"] },
        "claimsToVerify": { "type": "array", "items": { "type": "string" } }
      },
      "required": ["headline", "summary", "category", "importance",
                   "whyItMatters", "verificationStatus", "claimsToVerify"],
      "additionalProperties": false
    }
    """;
}
