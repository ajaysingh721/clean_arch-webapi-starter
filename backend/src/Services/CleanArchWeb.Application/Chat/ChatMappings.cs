using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using CleanArchWeb.Domain.Chat;

namespace CleanArchWeb.Application.Chat;

/// <summary>
/// Centralizes conversions between API DTOs and domain objects to keep controllers thin.
/// </summary>
public static class ChatMappings
{
    private const int MaxHistoryMessages = 20;

    public static ChatCompletionRequest ToDomain(this ChatCompletionRequestDto dto)
    {
        ArgumentNullException.ThrowIfNull(dto);
        var history = (dto.History ?? Array.Empty<ChatMessageDto>())
            .Take(MaxHistoryMessages)
            .Select(FromDto)
            .ToList();
        return ChatCompletionRequest.Create(dto.Prompt, history);
    }

    public static ChatCompletionResponseDto ToDto(this ChatCompletion completion)
    {
        ArgumentNullException.ThrowIfNull(completion);
        var conversationDtos = completion.Conversation.Select(ToDto).ToList();
        return new ChatCompletionResponseDto(
            ToDto(completion.AssistantMessage),
            conversationDtos,
            new ChatUsageDto(
                completion.PromptTokens,
                completion.CompletionTokens,
                completion.PromptTokens + completion.CompletionTokens),
            completion.Model,
            completion.CreatedUtc);
    }

    private static ChatMessage FromDto(ChatMessageDto dto)
    {
        var role = dto.Role?.Trim() ?? string.Empty;
        return ChatMessage.Create(ParseRole(role), dto.Content);
    }

    private static ChatMessageDto ToDto(ChatMessage message)
    {
        return new ChatMessageDto(message.Role.ToString().ToLower(CultureInfo.InvariantCulture), message.Content);
    }

    private static ChatRole ParseRole(string value)
    {
        if (Enum.TryParse<ChatRole>(value, true, out var role))
        {
            return role;
        }

        return ChatRole.User;
    }
}
