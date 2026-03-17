using Huntly.Core.Jobs.Entities;
using Huntly.Core.Jobs.Enums;
using Huntly.Core.Jobs.ValueObjects;
using Huntly.Core.Shared.Exceptions;
using NUnit.Framework;

namespace Huntly.Core.Tests.Jobs.Entities;

[TestFixture]
public class JobApplicationTests
{
    private Guid _userId;
    private CompanyName _companyName;
    private Position _position;

    [SetUp]
    public void SetUp()
    {
        _userId = Guid.NewGuid();
        _companyName = new CompanyName("Test Company");
        _position = new Position("Developer");
    }

    [Test]
    public void Create_ValidParameters_SetsPropertiesCorrectly()
    {
        var job = JobApplication.Create(_userId, _companyName, _position);

        Assert.That(job.UserId, Is.EqualTo(_userId));
        Assert.That(job.CompanyName, Is.EqualTo(_companyName));
        Assert.That(job.Position, Is.EqualTo(_position));
        Assert.That(job.Status, Is.EqualTo(ApplicationStatus.Applied));
        Assert.That(job.Interviews, Is.Empty);
        Assert.That(job.Notes, Is.Empty);
    }

    [Test]
    public void UpdateStatus_NewStatus_UpdatesStatusAndTimestamp()
    {
        var job = JobApplication.Create(_userId, _companyName, _position);
        var originalTimestamp = job.UpdatedAt;

        Thread.Sleep(50);
        job.UpdateStatus(ApplicationStatus.PhoneScreen);

        Assert.That(job.Status, Is.EqualTo(ApplicationStatus.PhoneScreen));
        Assert.That(job.UpdatedAt, Is.GreaterThan(originalTimestamp));
    }

    [Test]
    public void UpdateStatus_SameStatus_IgnoresTimestampUpdate()
    {
        var job = JobApplication.Create(_userId, _companyName, _position);
        var originalTimestamp = job.UpdatedAt;

        Thread.Sleep(50);
        job.UpdateStatus(ApplicationStatus.Applied); // Same status

        Assert.That(job.Status, Is.EqualTo(ApplicationStatus.Applied));
        Assert.That(job.UpdatedAt, Is.EqualTo(originalTimestamp));
    }

    [Test]
    public void AddInterview_OpenApplication_AddsInterviewToCollection()
    {
        var job = JobApplication.Create(_userId, _companyName, _position);

        var interview = job.AddInterview(InterviewType.HR, DateTime.UtcNow.AddDays(1), "Some notes");

        Assert.That(job.Interviews, Contains.Item(interview));
        Assert.That(job.Interviews.Count, Is.EqualTo(1));
    }

    [Test]
    public void AddInterview_ClosedApplication_ThrowsDomainException()
    {
        var job = JobApplication.Create(_userId, _companyName, _position);
        job.UpdateStatus(ApplicationStatus.Rejected);

        var ex = Assert.Throws<DomainException>(() => 
            job.AddInterview(InterviewType.HR, DateTime.UtcNow.AddDays(1), "Notes"));

        Assert.That(ex.Message, Is.EqualTo("Cannot add an interview to a closed application."));
    }

    [Test]
    public void AddInterview_ClosedApplicationButSkipCheck_AddsInterview()
    {
        var job = JobApplication.Create(_userId, _companyName, _position);
        job.UpdateStatus(ApplicationStatus.Rejected);

        var interview = job.AddInterview(InterviewType.HR, DateTime.UtcNow.AddDays(1), "Notes", InterviewOutcome.Pending, true);

        Assert.That(job.Interviews, Contains.Item(interview));
    }

    [Test]
    public void AddNote_AddsNoteToCollectionAndUpdatesTimestamp()
    {
        var job = JobApplication.Create(_userId, _companyName, _position);
        var originalTimestamp = job.UpdatedAt;

        Thread.Sleep(50);
        var note = job.AddNote("Some insight");

        Assert.That(job.Notes, Contains.Item(note));
        Assert.That(job.UpdatedAt, Is.GreaterThan(originalTimestamp));
    }

    [Test]
    public void RemoveInterview_ExistingInterview_RemovesSuccessfully()
    {
        var job = JobApplication.Create(_userId, _companyName, _position);
        var interview = job.AddInterview(InterviewType.HR, DateTime.UtcNow.AddDays(1), "Notes");

        var result = job.RemoveInterview(interview.Id);

        Assert.That(result, Is.True);
        Assert.That(job.Interviews, Is.Empty);
    }

    [Test]
    public void RecordInterviewOutcome_DelegatesToInterview()
    {
        var job = JobApplication.Create(_userId, _companyName, _position);
        var interview = job.AddInterview(InterviewType.HR, DateTime.UtcNow.AddDays(1), "Notes");

        var result = job.RecordInterviewOutcome(interview.Id, InterviewOutcome.Passed);

        Assert.That(result, Is.True);
        Assert.That(interview.Outcome, Is.EqualTo(InterviewOutcome.Passed));
    }

    [Test]
    public void RemoveNote_ExistingNote_RemovesSuccessfully()
    {
        var job = JobApplication.Create(_userId, _companyName, _position);
        var note = job.AddNote("Delete me");

        var result = job.RemoveNote(note.Id);

        Assert.That(result, Is.True);
        Assert.That(job.Notes, Is.Empty);
    }
}
