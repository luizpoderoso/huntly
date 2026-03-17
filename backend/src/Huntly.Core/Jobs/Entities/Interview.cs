using Huntly.Core.Jobs.Enums;
using Huntly.Core.Shared.Entities;
using Huntly.Core.Shared.Exceptions;

namespace Huntly.Core.Jobs.Entities;

public sealed class Interview : AuditableEntity
{
    public Guid JobApplicationId { get; private set; }
    public InterviewType Type { get; private set; }
    public DateTime ScheduledAt { get; private set; }
    public InterviewOutcome Outcome { get; private set; } = InterviewOutcome.Pending;
    public string? Notes { get; private set; }

    private Interview()
    {
    }

    internal static Interview Create(InterviewType type, DateTime scheduledAt, string? notes,
        InterviewOutcome outcome = InterviewOutcome.Pending)
    {
        if (scheduledAt < DateTime.UtcNow)
            throw new ArgumentException("Interview cannot be scheduled in the past.");

        return new Interview { Type = type, ScheduledAt = scheduledAt, Notes = notes, Outcome = outcome };
    }

    internal void RecordOutcome(InterviewOutcome outcome)
    {
        if (Outcome != InterviewOutcome.Pending)
            throw new DomainException("Outcome has already been recorded.");

        Outcome = outcome;
        UpdateTimestamp();
    }

    internal void ChangeNotes(string? notes)
    {
        if (Notes == notes)
            throw new DomainException("Notes are the same.");

        Notes = notes;
        UpdateTimestamp();
    }
}