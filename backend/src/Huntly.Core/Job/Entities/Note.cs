using Huntly.Core.Shared.Entities;

namespace Huntly.Core.Job.Entities;

public sealed class Note : AuditableEntity
{
    public Guid JobApplicationId { get; protected set; }
    public string Content { get; protected set; }
    
    private Note() {}

    public static Note Create(string content)
    {
        if (string.IsNullOrWhiteSpace(content))
            throw new ArgumentException("Note content cannot be empty.");
        if (content.Length > 2000)
            throw new ArgumentException("Note content cannot exceed 2000 characters.");

        return new Note { Content = content };
    }

    public void ChangeContent(string content)
    {
        Content = content;
        UpdateTimestamp();
    }
}