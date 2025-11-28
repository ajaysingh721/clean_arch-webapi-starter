using System;
using System.Collections.Generic;

namespace CleanArchWeb.Application.Chat;

/// <summary>
/// API response describing the assistant reply plus usage metadata.
/// </summary>
public sealed record ChatCompletionResponseDto(
    ChatMessageDto AssistantMessage,
    IReadOnlyList<ChatMessageDto> Conversation,
    ChatUsageDto Usage,
    string Model,
    DateTime CreatedUtc);

public sealed record ChatUsageDto(int PromptTokens, int CompletionTokens, int TotalTokens);
