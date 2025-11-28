using CleanArchWeb.Domain.Chat;
using CleanArchWeb.Infrastructure.Chat;
using FluentAssertions;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace CleanArchWeb.Api.Tests;

public sealed class MockChatCompletionServiceTests
{
    [Fact]
    public async Task CompleteAsync_WithPrompt_ReturnsAssistantReply()
    {
        var service = new MockChatCompletionService();
        var request = ChatCompletionRequest.Create(
            "Plan a chatbot sprint",
            new[] { ChatMessage.Create(ChatRole.User, "Remember to scope UI and API") });

        var completion = await service.CompleteAsync(request, CancellationToken.None);

        completion.AssistantMessage.Role.Should().Be(ChatRole.Assistant);
        completion.AssistantMessage.Content.Should().Contain("breakdown");
        completion.Conversation.Should().HaveCount(3);
        completion.Model.Should().Be("gpt-mock-1.0");
    }

    [Fact]
    public async Task CompleteAsync_SamePrompt_ProducesDeterministicOutput()
    {
        var service = new MockChatCompletionService();
        var request = ChatCompletionRequest.Create("Outline release process");

        var first = await service.CompleteAsync(request, CancellationToken.None);
        var second = await service.CompleteAsync(request, CancellationToken.None);

        first.AssistantMessage.Content.Should().Be(second.AssistantMessage.Content);
    }
}
