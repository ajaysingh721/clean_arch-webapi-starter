using System.Collections.Generic;
using System.Linq;
using System.Text;
using CleanArchWeb.Application.Chat;
using CleanArchWeb.Domain.Chat;

namespace CleanArchWeb.Infrastructure.Chat;

/// <summary>
/// Deterministic in-memory chat completion service that mimics an assistant for local development.
/// </summary>
public sealed class MockChatCompletionService : IChatCompletionService
{
    private const int MaxHistory = 20;
    private const string ModelName = "gpt-mock-1.0";

    private static readonly string[] CapabilitySnippets =
    [
        "summarizes the core idea",
        "highlights potential blockers",
        "suggests next steps",
        "adds guardrails for safety",
        "calls out data requirements"
    ];

    private static readonly string[] ToneSnippets =
    [
        "pragmatic",
        "supportive",
        "detailed",
        "curious",
        "candid"
    ];

    public Task<ChatCompletion> CompleteAsync(ChatCompletionRequest request, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(request);
        cancellationToken.ThrowIfCancellationRequested();

        var trimmedPrompt = request.Prompt.Trim();
        if (trimmedPrompt.Length == 0)
        {
            throw new ArgumentException("Prompt must have content.", nameof(request));
        }

        var sanitizedHistory = request.History.Take(MaxHistory).ToList();
        var assistantReply = BuildAssistantReply(trimmedPrompt, sanitizedHistory);
        var assistantMessage = ChatMessage.Create(ChatRole.Assistant, assistantReply);

        var conversation = new List<ChatMessage>(sanitizedHistory.Count + 2);
        conversation.AddRange(sanitizedHistory);
        conversation.Add(ChatMessage.Create(ChatRole.User, trimmedPrompt));
        conversation.Add(assistantMessage);

        var promptTokens = EstimateTokens(sanitizedHistory, trimmedPrompt);
        var completionTokens = EstimateTokens(assistantMessage);

        var completion = ChatCompletion.Create(
            assistantMessage,
            conversation,
            promptTokens,
            completionTokens,
            ModelName,
            DateTime.UtcNow);

        return Task.FromResult(completion);
    }

    private static string BuildAssistantReply(string prompt, IReadOnlyList<ChatMessage> history)
    {
        var seed = HashCode.Combine(prompt.ToLowerInvariant(), history.Count);
        var random = new Random(seed);
        var capability = CapabilitySnippets[random.Next(CapabilitySnippets.Length)];
        var tone = ToneSnippets[random.Next(ToneSnippets.Length)];

        var builder = new StringBuilder();
        builder.AppendLine($"Here is a {tone} breakdown tailored to your prompt.");
        builder.AppendLine();
        builder.AppendLine($"1. The assistant {capability} so you can take immediate action.");
        builder.AppendLine("2. Key context from the latest conversation turns is accounted for.");
        builder.AppendLine("3. Consider validating assumptions before implementation.");
        builder.AppendLine();
        builder.AppendLine($"Prompt focus: {Truncate(prompt, 240)}");

        return builder.ToString().Trim();
    }

    private static int EstimateTokens(IEnumerable<ChatMessage> messages, string? currentPrompt = null)
    {
        var total = messages.Sum(m => EstimateTokens(m));
        if (!string.IsNullOrWhiteSpace(currentPrompt))
        {
            total += Math.Max(1, currentPrompt.Split(' ', StringSplitOptions.RemoveEmptyEntries).Length);
        }

        return total * 4; // rough scalar for mock usage reporting
    }

    private static int EstimateTokens(ChatMessage message)
    {
        return Math.Max(1, message.Content.Split(' ', StringSplitOptions.RemoveEmptyEntries).Length) * 4;
    }

    private static string Truncate(string value, int maxLength)
    {
        if (value.Length <= maxLength)
        {
            return value;
        }

        return value[..maxLength] + "...";
    }
}
