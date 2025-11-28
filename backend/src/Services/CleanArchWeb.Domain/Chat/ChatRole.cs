namespace CleanArchWeb.Domain.Chat;

/// <summary>
/// Represents the speaker of a chat message to keep ubiquitous language consistent across layers.
/// </summary>
public enum ChatRole
{
    System = 0,
    User = 1,
    Assistant = 2
}
