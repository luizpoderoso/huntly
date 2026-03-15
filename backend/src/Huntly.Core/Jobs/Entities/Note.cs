using Huntly.Core.Shared.Entities;

namespace Huntly.Core.Jobs.Entities;

public sealed class Note : AuditableEntity
{
    public Guid JobApplicationId { get; private set; }
    public string Content { get; private set; }
    
    private Note() {}

    internal static Note Create(string content)
    {
        if (string.IsNullOrWhiteSpace(content))
            throw new ArgumentException("Note content cannot be empty.");
        if (content.Length > 2000)
            throw new ArgumentException("Note content cannot exceed 2000 characters.");

        return new Note { Content = content };
    }

    public void ChangeContent(string content)
    {
        if (string.IsNullOrWhiteSpace(content))
            throw new ArgumentException("Note content cannot be empty.");
        if (content.Length > 2000)
            throw new ArgumentException("Note content cannot exceed 2000 characters.");
        
        Content = content;
        UpdateTimestamp();
    }
}