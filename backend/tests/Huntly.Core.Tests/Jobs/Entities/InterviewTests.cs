using Huntly.Core.Jobs.Entities;
using Huntly.Core.Jobs.Enums;
using Huntly.Core.Shared.Exceptions;
using NUnit.Framework;

namespace Huntly.Core.Tests.Jobs.Entities;

[TestFixture]
public class InterviewTests
{
    [Test]
    public void Create_FutureDatePending_Succeeds()
    {
        var futureDate = DateTime.UtcNow.AddDays(1);
        var interview = Interview.Create(InterviewType.HR, futureDate, "HR intro");

        Assert.That(interview.ScheduledAt, Is.EqualTo(futureDate));
        Assert.That(interview.Outcome, Is.EqualTo(InterviewOutcome.Pending));
        Assert.That(interview.Notes, Is.EqualTo("HR intro"));
    }

    [Test]
    public void Create_PastDatePending_ThrowsArgumentException()
    {
        var pastDate = DateTime.UtcNow.AddDays(-1);

        var ex = Assert.Throws<ArgumentException>(() => Interview.Create(InterviewType.Technical, pastDate, "Test"));
        Assert.That(ex.Message, Is.EqualTo("Interview cannot be scheduled in the past."));
    }

    [Test]
    public void Create_PastDateCompleted_Succeeds()
    {
        var pastDate = DateTime.UtcNow.AddDays(-1);
        var interview = Interview.Create(InterviewType.Technical, pastDate, "Test", InterviewOutcome.Passed);

        Assert.That(interview.ScheduledAt, Is.EqualTo(pastDate));
        Assert.That(interview.Outcome, Is.EqualTo(InterviewOutcome.Passed));
    }

    [Test]
    public void RecordOutcome_WasPending_RecordsAndUpdatesTimestamp()
    {
        var interview = Interview.Create(InterviewType.HR, DateTime.UtcNow.AddDays(1), "HR intro");
        var originalTimestamp = interview.UpdatedAt;
        
        Thread.Sleep(50);
        interview.RecordOutcome(InterviewOutcome.Passed);

        Assert.That(interview.Outcome, Is.EqualTo(InterviewOutcome.Passed));
        Assert.That(interview.UpdatedAt, Is.GreaterThan(originalTimestamp));
    }

    [Test]
    public void RecordOutcome_WasAlreadyRecorded_ThrowsDomainException()
    {
        var interview = Interview.Create(InterviewType.HR, DateTime.UtcNow.AddDays(1), "HR intro");
        interview.RecordOutcome(InterviewOutcome.Passed);

        var ex = Assert.Throws<DomainException>(() => interview.RecordOutcome(InterviewOutcome.Failed));
        Assert.That(ex.Message, Is.EqualTo("Outcome has already been recorded."));
    }

    [Test]
    public void ChangeNotes_NewNotes_UpdatesAndBumpsTimestamp()
    {
        var interview = Interview.Create(InterviewType.HR, DateTime.UtcNow.AddDays(1), "HR intro");
        var originalTimestamp = interview.UpdatedAt;

        Thread.Sleep(50);
        interview.ChangeNotes("New HR intro notes");

        Assert.That(interview.Notes, Is.EqualTo("New HR intro notes"));
        Assert.That(interview.UpdatedAt, Is.GreaterThan(originalTimestamp));
    }

    [Test]
    public void ChangeNotes_SameNotes_ThrowsDomainException()
    {
        var interview = Interview.Create(InterviewType.HR, DateTime.UtcNow.AddDays(1), "HR intro");

        var ex = Assert.Throws<DomainException>(() => interview.ChangeNotes("HR intro"));
        Assert.That(ex.Message, Is.EqualTo("Notes are the same."));
    }
}
