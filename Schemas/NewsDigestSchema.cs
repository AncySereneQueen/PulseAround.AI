namespace PulseAround.AI.Schemas;

public static class NewsDigestSchema
{
    public const string Json = """
    {
      "type": "object",
      "properties": {
        "items": {
          "type": "array",
          "items": {
            "type": "object",
            "properties": {
              "sourceUrl": { "type": "string" },
              "analysis": {
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
                "required": ["headline", "summary", "category", "importance", "whyItMatters", "verificationStatus", "claimsToVerify"],
                "additionalProperties": false
              }
            },
            "required": ["sourceUrl", "analysis"],
            "additionalProperties": false
          }
        }
      },
      "required": ["items"],
      "additionalProperties": false
    }
    """;
}
