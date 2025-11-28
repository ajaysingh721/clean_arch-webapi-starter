using System.Collections.Generic;

namespace CleanArchWeb.Application.Chat;

/// <summary>
/// API request payload for generating a chat completion.
/// </summary>
public sealed record ChatCompletionRequestDto(string Prompt, IReadOnlyList<ChatMessageDto>? History);
