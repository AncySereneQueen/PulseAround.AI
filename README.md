# PulseAround.AI — RSS + Cached AI Analysis

.NET 10 console app. RSS news is collected from eight predefined sources, filtered and cached. The OpenAI agent analyzes news and displays headline, summary, category, importance, why it matters, verification status (`NotVerified`), claims to verify, and source.

## Commands

- `dotnet build`: compile, no API charges.
- `dotnet run`: show cached AI analysis for matching articles; show RSS details for articles not yet analyzed. **No API charges**.
- `dotnet run -- --refresh`: fetch fresh RSS; reuse cached AI analyses only for unchanged articles. **No API charges**.
- `dotnet run -- --ai`: reuse cached analyses and request AI analysis for uncached selected articles. **May incur API charges**. A fully cached selection does not call OpenAI.

## Cache

`bin/Debug/net10.0/Cache/news-cache.json` stores RSS articles; `analysis-cache.json` stores AI results, keyed by source URL and a SHA-256 fingerprint of title, description, category, and URL. Updated article content invalidates its old analysis. The agent may select up to 12 stories for a single response; not every RSS article will necessarily receive analysis in one run. The cache is local to the build output directory and is not shared across configurations.

No global web search or independent verification is performed. `VerificationStatus` remains `NotVerified`. Do not commit API keys. Set `OPENAI_PULSE_API_KEY` only when AI calls are needed.
