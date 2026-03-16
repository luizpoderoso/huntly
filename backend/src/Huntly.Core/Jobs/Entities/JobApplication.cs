using Huntly.Core.Jobs.Enums;
using Huntly.Core.Jobs.ValueObjects;
using Huntly.Core.Shared.Entities;
using Huntly.Core.Shared.Exceptions;

namespace Huntly.Core.Jobs.Entities;

public sealed class JobApplication : AuditableEntity
{
    public Guid UserId { get; private set; }
    public CompanyName CompanyName { get; private set; }
    public Position Position { get; private set; }
    public ApplicationStatus Status { get; private set; } = ApplicationStatus.Applied;
    public JobUrl? JobUrl { get; private set; }
    public SalaryRange? SalaryRange { get; private set; }
    
    private readonly List<Interview> _interviews = new();
    private readonly List<Note> _notes = new();
    public IReadOnlyCollection<Interview> Interviews => _interviews.AsReadOnly();
    public IReadOnlyCollection<Note> Notes => _notes.AsReadOnly();
    
    private JobApplication() {}

    public static JobApplication Create(
        Guid userId,
        CompanyName companyName,
        Position position,
        JobUrl? jobUrl = null,
        SalaryRange? salaryRange = null)
    {
        var job = new JobApplication
        {
            UserId = userId,
            CompanyName = companyName,
            Position = position,
            Status = ApplicationStatus.Applied,
            JobUrl = jobUrl,
            SalaryRange = salaryRange
        };

        return job;
    }
    
    public void ChangeCompanyName(CompanyName companyName)
    {
        CompanyName = companyName;
        UpdateTimestamp();
    }

    public void ChangePosition(Position position)
    {
        Position = position;
        UpdateTimestamp();
    }

    public void UpdateStatus(ApplicationStatus newStatus)
    {
        if (Status == newStatus) return;
        Status = newStatus;
        UpdateTimestamp();
    }
    
    public Interview AddInterview(InterviewType type, DateTime scheduledAt, string? interviewNotes)
    {
        if (Status == ApplicationStatus.Rejected || Status == ApplicationStatus.Withdrawn)
            throw new DomainException("Cannot add an interview to a closed application.");

        var interview = Interview.Create(type, scheduledAt, interviewNotes);

        _interviews.Add(interview);
        UpdateTimestamp();

        return interview;
    }
    
    public bool RemoveInterview(Guid interviewId)
    {
        var interview = _interviews.FirstOrDefault(i => i.Id == interviewId);

        if (interview is null)
            return false;
    
        _interviews.Remove(interview);
        UpdateTimestamp();
        return true;
    }

    public bool RecordInterviewOutcome(Guid interviewId, InterviewOutcome newOutcome)
    {
        var interview = _interviews.FirstOrDefault(i => i.Id == interviewId);
        
        if (interview is null)
            return false;
        
        interview.RecordOutcome(newOutcome);
        UpdateTimestamp();
        return true;
    }

    public bool ChangeInterviewNotes(Guid interviewId, string newNotes)
    {
        var interview = _interviews.FirstOrDefault(i => i.Id == interviewId);
        
        if (interview is null)
            return false;
        
        interview.ChangeNotes(newNotes);
        UpdateTimestamp();
        return true;
    }

    public Note AddNote(string content)
    {
        var note = Note.Create(content);
        _notes.Add(note);
        UpdateTimestamp();
        return note;
    }

    public bool RemoveNote(Guid noteId)
    {
        var note = _notes.FirstOrDefault(n => n.Id == noteId);

        if (note is null)
            return false;

        _notes.Remove(note);
        UpdateTimestamp();
        return true;
    }

    public bool ChangeNoteContent(Guid noteId, string newContent)
    {
        var note = _notes.FirstOrDefault(n => n.Id == noteId);

        if (note is null)
            return false;

        note.ChangeContent(newContent);
        UpdateTimestamp();
        return true;
    }
}