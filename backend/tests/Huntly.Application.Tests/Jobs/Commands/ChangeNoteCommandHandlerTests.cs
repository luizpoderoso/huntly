using Huntly.Application.Jobs.Commands.ChangeNote;
using Huntly.Application.Shared.Exceptions;
using Huntly.Application.Shared.Interfaces;
using Huntly.Core.Jobs.Entities;
using Huntly.Core.Jobs.Repositories;
using Huntly.Core.Jobs.ValueObjects;
using NSubstitute;
using NSubstitute.ReturnsExtensions;
using NUnit.Framework;

namespace Huntly.Application.Tests.Jobs.Commands;

[TestFixture]
public class ChangeNoteCommandHandlerTests
{
    private IJobApplicationRepository _repository;
    private IAtomicWork _atomicWork;
    private IUserContext _userContext;
    private ChangeNoteCommandHandler _handler;

    [SetUp]
    public void SetUp()
    {
        _repository = Substitute.For<IJobApplicationRepository>();
        _atomicWork = Substitute.For<IAtomicWork>();
        _userContext = Substitute.For<IUserContext>();

        _userContext.UserId.Returns(Guid.Parse("88888888-8888-8888-8888-888888888888"));

        _handler = new ChangeNoteCommandHandler(_repository, _atomicWork, _userContext);
    }

    [Test]
    public async Task Handle_ValidCommand_ChangesNoteSuccessfully()
    {
        var jobId = Guid.NewGuid();
        var job = JobApplication.Create(_userContext.UserId, new CompanyName("Company A"), new Position("Dev"));
        var note = job.AddNote("Old Note");

        var command = new ChangeNoteCommand(jobId, note.Id, "New Note");

        _repository.GetByIdAsync(jobId, Arg.Any<CancellationToken>()).Returns(job);

        await _handler.Handle(command, CancellationToken.None).AsTask();

        Assert.That(note.Content, Is.EqualTo("New Note"));
        await _atomicWork.Received(1).CommitAsync(Arg.Any<CancellationToken>());
    }

    [Test]
    public void Handle_JobNotFound_ThrowsNotFoundException()
    {
        var command = new ChangeNoteCommand(Guid.NewGuid(), Guid.NewGuid(), "New Note");
        _repository.GetByIdAsync(command.JobApplicationId, Arg.Any<CancellationToken>()).ReturnsNull();

        var ex = Assert.ThrowsAsync<NotFoundException>(() => _handler.Handle(command, CancellationToken.None).AsTask());
        Assert.That(ex.Message, Is.EqualTo("Job application not found."));
    }

    [Test]
    public void Handle_JobBelongsToDifferentUser_ThrowsNotFoundException()
    {
        var jobId = Guid.NewGuid();
        var command = new ChangeNoteCommand(jobId, Guid.NewGuid(), "New Note");

        var job = JobApplication.Create(Guid.NewGuid(), new CompanyName("Other User's Job"), new Position("Dev"));
        _repository.GetByIdAsync(jobId, Arg.Any<CancellationToken>()).Returns(job);

        var ex = Assert.ThrowsAsync<NotFoundException>(() => _handler.Handle(command, CancellationToken.None).AsTask());
        Assert.That(ex.Message, Is.EqualTo("Job application not found."));
    }

    [Test]
    public void Handle_NoteNotFound_ThrowsNotFoundException()
    {
        var jobId = Guid.NewGuid();
        var command = new ChangeNoteCommand(jobId, Guid.NewGuid(), "New Note");

        var job = JobApplication.Create(_userContext.UserId, new CompanyName("Company A"), new Position("Dev"));
        _repository.GetByIdAsync(jobId, Arg.Any<CancellationToken>()).Returns(job);

        var ex = Assert.ThrowsAsync<NotFoundException>(() => _handler.Handle(command, CancellationToken.None).AsTask());
        Assert.That(ex.Message, Is.EqualTo("Note not found."));
    }
}
