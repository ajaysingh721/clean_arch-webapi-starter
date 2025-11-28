using System;

namespace CleanArchWeb.Domain.Chat;

/// <summary>
/// Immutable value object describing a single utterance within a chat session.
/// </summary>
public sealed class ChatMessage
{
    public ChatRole Role { get; }
    public string Content { get; }

    private ChatMessage(ChatRole role, string content)
    {
        Role = role;
        Content = content;
    }

    public static ChatMessage Create(ChatRole role, string content)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(content, nameof(content));
        return new ChatMessage(role, content.Trim());
    }
}
