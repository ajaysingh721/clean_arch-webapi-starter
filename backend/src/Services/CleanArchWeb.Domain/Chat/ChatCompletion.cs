using System;
using System.Collections.Generic;

namespace CleanArchWeb.Domain.Chat;

/// <summary>
/// Captures the assistant reply alongside conversation metadata for auditing.
/// </summary>
public sealed class ChatCompletion
{
    private ChatCompletion(
        ChatMessage assistantMessage,
        IReadOnlyList<ChatMessage> conversation,
        int promptTokens,
        int completionTokens,
        string model,
        DateTime createdUtc)
    {
        AssistantMessage = assistantMessage;
        Conversation = conversation;
        PromptTokens = promptTokens;
        CompletionTokens = completionTokens;
        Model = model;
        CreatedUtc = createdUtc;
    }

    public ChatMessage AssistantMessage { get; }

    public IReadOnlyList<ChatMessage> Conversation { get; }

    public int PromptTokens { get; }

    public int CompletionTokens { get; }

    public string Model { get; }

    public DateTime CreatedUtc { get; }

    public static ChatCompletion Create(
        ChatMessage assistantMessage,
        IReadOnlyList<ChatMessage> conversation,
        int promptTokens,
        int completionTokens,
        string model,
        DateTime? createdUtc = null)
    {
        ArgumentNullException.ThrowIfNull(assistantMessage);
        ArgumentNullException.ThrowIfNull(conversation);
        ArgumentException.ThrowIfNullOrWhiteSpace(model, nameof(model));
        if (promptTokens < 0)
        {
            throw new ArgumentOutOfRangeException(nameof(promptTokens));
        }

        if (completionTokens < 0)
        {
            throw new ArgumentOutOfRangeException(nameof(completionTokens));
        }

        var timestamp = createdUtc ?? DateTime.UtcNow;
        return new ChatCompletion(
            assistantMessage,
            conversation,
            promptTokens,
            completionTokens,
            model.Trim(),
            timestamp);
    }
}
