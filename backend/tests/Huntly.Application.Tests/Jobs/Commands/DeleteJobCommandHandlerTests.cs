using Huntly.Application.Jobs.Commands.DeleteJob;
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
public class DeleteJobCommandHandlerTests
{
    private IJobApplicationRepository _repository;
    private IAtomicWork _atomicWork;
    private IUserContext _userContext;
    private DeleteJobCommandHandler _handler;

    [SetUp]
    public void SetUp()
    {
        _repository = Substitute.For<IJobApplicationRepository>();
        _atomicWork = Substitute.For<IAtomicWork>();
        _userContext = Substitute.For<IUserContext>();

        _userContext.UserId.Returns(Guid.Parse("11111111-1111-1111-1111-111111111111"));

        _handler = new DeleteJobCommandHandler(_repository, _atomicWork, _userContext);
    }

    [Test]
    public async Task Handle_ValidCommand_DeletesJobSuccessfully()
    {
        // Arrange
        var jobId = Guid.NewGuid();
        var command = new DeleteJobCommand(jobId);
        
        var job = JobApplication.Create(
            _userContext.UserId,
            new CompanyName("To Delete"),
            new Position("Dev")
        );

        _repository.GetByIdAsync(jobId, Arg.Any<CancellationToken>()).Returns(job);

        // Act
        await _handler.Handle(command, CancellationToken.None).AsTask();

        // Assert
        _repository.Received(1).Remove(job);
        await _atomicWork.Received(1).CommitAsync(Arg.Any<CancellationToken>());
    }

    [Test]
    public void Handle_JobNotFound_ThrowsNotFoundException()
    {
        // Arrange
        var command = new DeleteJobCommand(Guid.NewGuid());

        _repository.GetByIdAsync(command.JobId, Arg.Any<CancellationToken>()).ReturnsNull();

        // Act & Assert
        Assert.ThrowsAsync<NotFoundException>(() => _handler.Handle(command, CancellationToken.None).AsTask());
    }

    [Test]
    public void Handle_JobBelongsToDifferentUser_ThrowsNotFoundException()
    {
        // Arrange
        var jobId = Guid.NewGuid();
        var command = new DeleteJobCommand(jobId);
        
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
