using Huntly.Core.Job.Enums;
using Huntly.Core.Job.ValueObjects;
using Huntly.Core.Shared.Entities;
using Huntly.Core.Shared.Exceptions;

namespace Huntly.Core.Job.Entities;

public class JobApplication : AuditableEntity
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
    
    public void AddInterview(InterviewType type, DateTime scheduledAt, string? interviewNotes)
    {
        if (Status == ApplicationStatus.Rejected || Status == ApplicationStatus.Withdrawn)
            throw new DomainException("Cannot add an interview to a closed application.");

        _interviews.Add(Interview.Create(type, scheduledAt, interviewNotes));
        UpdateTimestamp();
    }

    public void AddNote(string content)
    {
        _notes.Add(Note.Create(content));
        UpdateTimestamp();
    }
}