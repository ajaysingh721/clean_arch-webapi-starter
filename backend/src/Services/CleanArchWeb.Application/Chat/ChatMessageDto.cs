namespace CleanArchWeb.Application.Chat;

/// <summary>
/// Transport-friendly representation of a chat message used by the API layer.
/// </summary>
public sealed record ChatMessageDto(string Role, string Content);
