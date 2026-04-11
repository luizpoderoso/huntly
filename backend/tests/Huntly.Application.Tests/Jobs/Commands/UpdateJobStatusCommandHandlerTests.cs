using Huntly.Application.Jobs.Commands.UpdateJobStatus;
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
public class UpdateJobStatusCommandHandlerTests
{
    private IJobApplicationRepository _repository;
    private IAtomicWork _atomicWork;
    private IUserContext _userContext;
    private UpdateJobStatusCommandHandler _handler;

    [SetUp]
    public void SetUp()
    {
        _repository = Substitute.For<IJobApplicationRepository>();
        _atomicWork = Substitute.For<IAtomicWork>();
        _userContext = Substitute.For<IUserContext>();

        _userContext.UserId.Returns(Guid.Parse("22222222-2222-2222-2222-222222222222"));

        _handler = new UpdateJobStatusCommandHandler(_repository, _atomicWork, _userContext);
    }

    [Test]
    public async Task Handle_ValidCommand_UpdatesStatusSuccessfully()
    {
        // Arrange
        var jobId = Guid.NewGuid();
        var command = new UpdateJobStatusCommand(jobId, ApplicationStatus.TechnicalInterview);
        
        var job = JobApplication.Create(
            _userContext.UserId,
            new CompanyName("To Update"),
            new Position("Dev")
        );

        _repository.GetByIdAsync(jobId, Arg.Any<CancellationToken>()).Returns(job);

        // Act
        await _handler.Handle(command, CancellationToken.None).AsTask();

        // Assert
        Assert.That(job.Status, Is.EqualTo(ApplicationStatus.TechnicalInterview));
        await _atomicWork.Received(1).CommitAsync(Arg.Any<CancellationToken>());
    }

    [Test]
    public void Handle_JobNotFound_ThrowsNotFoundException()
    {
        // Arrange
        var command = new UpdateJobStatusCommand(Guid.NewGuid(), ApplicationStatus.TechnicalInterview);

        _repository.GetByIdAsync(command.JobId, Arg.Any<CancellationToken>()).ReturnsNull();

        // Act & Assert
        Assert.ThrowsAsync<NotFoundException>(() => _handler.Handle(command, CancellationToken.None).AsTask());
    }

    [Test]
    public void Handle_JobBelongsToDifferentUser_ThrowsNotFoundException()
    {
        // Arrange
        var jobId = Guid.NewGuid();
        var command = new UpdateJobStatusCommand(jobId, ApplicationStatus.TechnicalInterview);
        
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
