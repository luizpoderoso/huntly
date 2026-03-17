using Huntly.Application.Jobs.Commands.AddInterview;
using Huntly.Application.Shared.Exceptions;
using Huntly.Application.Shared.Interfaces;
using Huntly.Core.Jobs.Entities;
using Huntly.Core.Jobs.Enums;
using Huntly.Core.Jobs.Repositories;
using Huntly.Core.Jobs.ValueObjects;
using NSubstitute;
using NSubstitute.ReturnsExtensions;
using NUnit.Framework;

namespace Huntly.Application.Tests.Jobs.Commands;

[TestFixture]
public class AddInterviewCommandHandlerTests
{
    private IJobApplicationRepository _repository;
    private IAtomicWork _atomicWork;
    private IUserContext _userContext;
    private AddInterviewCommandHandler _handler;

    [SetUp]
    public void SetUp()
    {
        _repository = Substitute.For<IJobApplicationRepository>();
        _atomicWork = Substitute.For<IAtomicWork>();
        _userContext = Substitute.For<IUserContext>();

        _userContext.UserId.Returns(Guid.Parse("33333333-3333-3333-3333-333333333333"));

        _handler = new AddInterviewCommandHandler(_repository, _userContext, _atomicWork);
    }

    [Test]
    public async Task Handle_ValidCommand_AddsInterviewSuccessfully()
    {
        // Arrange
        var jobId = Guid.NewGuid();
        var scheduledAt = DateTime.UtcNow.AddDays(2);
        var command = new AddInterviewCommand(jobId, InterviewType.Technical, scheduledAt, "Be prepared!");

        var job = JobApplication.Create(
            _userContext.UserId,
            new CompanyName("To Add Interview"),
            new Position("Dev")
        );

        _repository.GetByIdAsync(jobId, Arg.Any<CancellationToken>()).Returns(job);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.That(result, Is.Not.Null);
        Assert.That(result.Type, Is.EqualTo(InterviewType.Technical.ToString()));
        Assert.That(result.ScheduledAt, Is.EqualTo(scheduledAt));
        Assert.That(result.Notes, Is.EqualTo("Be prepared!"));
        
        await _repository.Received(1).AddInterviewAsync(Arg.Is<Interview>(i => i.Id == result.Id), Arg.Any<CancellationToken>());
        await _atomicWork.Received(1).CommitAsync(Arg.Any<CancellationToken>());
    }

    [Test]
    public void Handle_JobNotFound_ThrowsNotFoundException()
    {
        // Arrange
        var command = new AddInterviewCommand(Guid.NewGuid(), InterviewType.HR, DateTime.UtcNow, null);

        _repository.GetByIdAsync(command.JobApplicationId, Arg.Any<CancellationToken>()).ReturnsNull();

        // Act & Assert
        Assert.ThrowsAsync<NotFoundException>(() => _handler.Handle(command, CancellationToken.None));
    }

    [Test]
    public void Handle_JobBelongsToDifferentUser_ThrowsNotFoundException()
    {
        // Arrange
        var jobId = Guid.NewGuid();
        var command = new AddInterviewCommand(jobId, InterviewType.Cultural, DateTime.UtcNow, "Different user");
        
        var job = JobApplication.Create(
            Guid.Parse("99999999-9999-9999-9999-999999999999"), // Different UserId
            new CompanyName("Other User's Job"),
            new Position("Dev")
        );

        _repository.GetByIdAsync(jobId, Arg.Any<CancellationToken>()).Returns(job);

        // Act & Assert
        Assert.ThrowsAsync<NotFoundException>(() => _handler.Handle(command, CancellationToken.None));
    }
}
