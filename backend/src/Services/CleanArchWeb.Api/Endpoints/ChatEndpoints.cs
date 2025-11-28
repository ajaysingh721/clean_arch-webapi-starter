using CleanArchWeb.Application.Chat;
using Microsoft.AspNetCore.Http.HttpResults;

namespace CleanArchWeb.Api.Endpoints;

/// <summary>
/// Minimal API endpoints for interacting with the mock chat completion service.
/// </summary>
public static class ChatEndpoints
{
    public static IEndpointRouteBuilder MapChatEndpoints(this IEndpointRouteBuilder routes)
    {
        var group = routes.MapGroup("/api/chat");

        group.MapPost("/completions", async Task<Results<BadRequest<string>, Ok<ChatCompletionResponseDto>>>
        (
            ChatCompletionRequestDto requestDto,
            IChatCompletionService service,
            CancellationToken cancellationToken) =>
        {
            if (requestDto is null)
            {
                return TypedResults.BadRequest("Request body is required.");
            }

            if (string.IsNullOrWhiteSpace(requestDto.Prompt))
            {
                return TypedResults.BadRequest("Prompt must not be empty.");
            }

            var domainRequest = requestDto.ToDomain();
            var completion = await service.CompleteAsync(domainRequest, cancellationToken);
            return TypedResults.Ok(completion.ToDto());
        })
        .WithName("CreateChatCompletion");

        return routes;
    }
}
