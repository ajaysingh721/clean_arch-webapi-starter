using System.Collections.Generic;
using System.Net;
using System.Net.Http.Json;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc.Testing;
using Xunit;

namespace CleanArchWeb.Api.IntegrationTests;

public class ChatEndpointTests : IClassFixture<WebApplicationFactory<Program>>
{
    private readonly WebApplicationFactory<Program> _factory;

    public ChatEndpointTests(WebApplicationFactory<Program> factory)
    {
        _factory = factory.WithWebHostBuilder(_ => { });
    }

    [Fact]
    public async Task CreateChatCompletion_WithValidPrompt_ReturnsAssistantMessage()
    {
        var client = _factory.CreateClient();
        var payload = new ChatRequest("Draft onboarding flow", new List<ChatMessage>());

        var response = await client.PostAsJsonAsync("/api/chat/completions", payload);

        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var body = await response.Content.ReadFromJsonAsync<ChatResponse>();
        body.Should().NotBeNull();
        body!.AssistantMessage.Role.Should().Be("assistant");
        body.AssistantMessage.Content.Should().Contain("breakdown");
        body.Usage.TotalTokens.Should().BeGreaterThan(0);
    }

    private sealed record ChatRequest(string Prompt, IReadOnlyList<ChatMessage> History);
    private sealed record ChatMessage(string Role, string Content);
    private sealed record ChatResponse(ChatMessage AssistantMessage, IReadOnlyList<ChatMessage> Conversation, UsageDto Usage, string Model, DateTime CreatedUtc);
    private sealed record UsageDto(int PromptTokens, int CompletionTokens, int TotalTokens);
}
