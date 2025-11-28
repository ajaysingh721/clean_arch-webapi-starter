using System.Threading;
using System.Threading.Tasks;
using CleanArchWeb.Domain.Chat;

namespace CleanArchWeb.Application.Chat;

/// <summary>
/// Provides chat completions without exposing infrastructure concerns to the API layer.
/// </summary>
public interface IChatCompletionService
{
    Task<ChatCompletion> CompleteAsync(ChatCompletionRequest request, CancellationToken cancellationToken = default);
}
