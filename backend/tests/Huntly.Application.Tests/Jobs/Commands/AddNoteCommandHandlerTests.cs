using Huntly.Application.Jobs.Commands.AddNote;
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
public class AddNoteCommandHandlerTests
{
    private IJobApplicationRepository _repository;
    private IAtomicWork _atomicWork;
    private IUserContext _userContext;
    private AddNoteCommandHandler _handler;

    [SetUp]
    public void SetUp()
    {
        _repository = Substitute.For<IJobApplicationRepository>();
        _atomicWork = Substitute.For<IAtomicWork>();
        _userContext = Substitute.For<IUserContext>();

        _userContext.UserId.Returns(Guid.Parse("44444444-4444-4444-4444-444444444444"));

        _handler = new AddNoteCommandHandler(_repository, _atomicWork, _userContext);
    }

    [Test]
    public async Task Handle_ValidCommand_AddsNoteSuccessfully()
    {
        // Arrange
        var jobId = Guid.NewGuid();
        var command = new AddNoteCommand(jobId, "Great interview today!");

        var job = JobApplication.Create(
            _userContext.UserId,
            new CompanyName("To Add Note"),
            new Position("Dev")
        );

        _repository.GetByIdAsync(jobId, Arg.Any<CancellationToken>()).Returns(job);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None).AsTask();

        // Assert
        Assert.That(result, Is.Not.Null);
        Assert.That(result.Content, Is.EqualTo("Great interview today!"));
        
        await _repository.Received(1).AddNoteAsync(Arg.Is<Note>(n => n.Id == result.Id), Arg.Any<CancellationToken>());
        await _atomicWork.Received(1).CommitAsync(Arg.Any<CancellationToken>());
    }

    [Test]
    public void Handle_JobNotFound_ThrowsNotFoundException()
    {
        // Arrange
        var command = new AddNoteCommand(Guid.NewGuid(), "It doesn't exist");

        _repository.GetByIdAsync(command.JobApplicationId, Arg.Any<CancellationToken>()).ReturnsNull();

        // Act & Assert
        Assert.ThrowsAsync<NotFoundException>(() => _handler.Handle(command, CancellationToken.None).AsTask());
    }

    [Test]
    public void Handle_JobBelongsToDifferentUser_ThrowsNotFoundException()
    {
        // Arrange
        var jobId = Guid.NewGuid();
        var command = new AddNoteCommand(jobId, "Cannot add note here");
        
        var job = JobApplication.Create(
            Guid.Parse("99999999-9999-9999-9999-999999999999"), // Different UserId
            new CompanyName("Other User's Job"),
            new Position("Dev")
        );

        _repository.GetByIdAsync(jobId, Arg.Any<CancellationToken>()).Returns(job);

        // Act & Assert
        Assert.ThrowsAsync<NotFoundException>(() => _handler.Handle(command, CancellationToken.None).AsTask());
    }
}
