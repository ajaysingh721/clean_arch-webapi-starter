using System.Collections.Generic;
using System.Linq;

namespace CleanArchWeb.Domain.Chat;

/// <summary>
/// Defines the user prompt and optional history required to construct a completion.
/// </summary>
public sealed class ChatCompletionRequest
{
    private ChatCompletionRequest(string prompt, IReadOnlyList<ChatMessage> history)
    {
        Prompt = prompt;
        History = history;
    }

    public string Prompt { get; }

    public IReadOnlyList<ChatMessage> History { get; }

    public static ChatCompletionRequest Create(string prompt, IEnumerable<ChatMessage>? history = null)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(prompt, nameof(prompt));
        var trimmedPrompt = prompt.Trim();
        var historyList = history?.ToList() ?? new List<ChatMessage>();
        return new ChatCompletionRequest(trimmedPrompt, historyList);
    }
}
