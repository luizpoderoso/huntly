using Huntly.Application.Jobs.Commands.DeleteInterview;
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
public class DeleteInterviewCommandHandlerTests
{
    private IJobApplicationRepository _repository;
    private IAtomicWork _atomicWork;
    private IUserContext _userContext;
    private DeleteInterviewCommandHandler _handler;

    [SetUp]
    public void SetUp()
    {
        _repository = Substitute.For<IJobApplicationRepository>();
        _atomicWork = Substitute.For<IAtomicWork>();
        _userContext = Substitute.For<IUserContext>();

        _userContext.UserId.Returns(Guid.Parse("99999999-9999-9999-9999-999999999999"));

        _handler = new DeleteInterviewCommandHandler(_repository, _atomicWork, _userContext);
    }

    [Test]
    public async Task Handle_ValidCommand_DeletesInterviewSuccessfully()
    {
        var jobId = Guid.NewGuid();
        var job = JobApplication.Create(_userContext.UserId, new CompanyName("Company A"), new Position("Dev"));
        var interview = job.AddInterview(InterviewType.HR, DateTime.UtcNow, "Delete me");

        var command = new DeleteInterviewCommand(jobId, interview.Id);

        _repository.GetByIdAsync(jobId, Arg.Any<CancellationToken>()).Returns(job);

        await _handler.Handle(command, CancellationToken.None).AsTask();

        Assert.That(job.Interviews, Is.Empty);
        await _atomicWork.Received(1).CommitAsync(Arg.Any<CancellationToken>());
    }

    [Test]
    public void Handle_JobNotFound_ThrowsNotFoundException()
    {
        var command = new DeleteInterviewCommand(Guid.NewGuid(), Guid.NewGuid());
        _repository.GetByIdAsync(command.JobApplicationId, Arg.Any<CancellationToken>()).ReturnsNull();

        var ex = Assert.ThrowsAsync<NotFoundException>(() => _handler.Handle(command, CancellationToken.None).AsTask());
        Assert.That(ex.Message, Is.EqualTo("Job application not found."));
    }

    [Test]
    public void Handle_JobBelongsToDifferentUser_ThrowsNotFoundException()
    {
        var jobId = Guid.NewGuid();
        var command = new DeleteInterviewCommand(jobId, Guid.NewGuid());

        var job = JobApplication.Create(Guid.NewGuid(), new CompanyName("Other User's Job"), new Position("Dev"));
        _repository.GetByIdAsync(jobId, Arg.Any<CancellationToken>()).Returns(job);

        var ex = Assert.ThrowsAsync<NotFoundException>(() => _handler.Handle(command, CancellationToken.None).AsTask());
        Assert.That(ex.Message, Is.EqualTo("Job application not found."));
    }

    [Test]
    public void Handle_InterviewNotFound_ThrowsNotFoundException()
    {
        var jobId = Guid.NewGuid();
        var command = new DeleteInterviewCommand(jobId, Guid.NewGuid());

        var job = JobApplication.Create(_userContext.UserId, new CompanyName("Company A"), new Position("Dev"));
        _repository.GetByIdAsync(jobId, Arg.Any<CancellationToken>()).Returns(job);

        var ex = Assert.ThrowsAsync<NotFoundException>(() => _handler.Handle(command, CancellationToken.None).AsTask());
        Assert.That(ex.Message, Is.EqualTo("Interview not found."));
    }
}
