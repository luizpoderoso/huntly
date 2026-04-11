using Huntly.Application.Jobs.Queries.GetJobById;
using Huntly.Application.Shared.Exceptions;
using Huntly.Application.Shared.Interfaces;
using Huntly.Core.Jobs.Entities;
using Huntly.Core.Jobs.Repositories;
using Huntly.Core.Jobs.ValueObjects;
using NSubstitute;
using NSubstitute.ReturnsExtensions;
using NUnit.Framework;

namespace Huntly.Application.Tests.Jobs.Queries;

[TestFixture]
public class GetJobByIdQueryHandlerTests
{
    private IJobApplicationRepository _repository;
    private IUserContext _userContext;
    private GetJobByIdQueryHandler _handler;

    [SetUp]
    public void SetUp()
    {
        _repository = Substitute.For<IJobApplicationRepository>();
        _userContext = Substitute.For<IUserContext>();

        _userContext.UserId.Returns(Guid.Parse("66666666-6666-6666-6666-666666666666"));

        _handler = new GetJobByIdQueryHandler(_repository, _userContext);
    }

    [Test]
    public async Task Handle_ValidQuery_ReturnsMappedJobDetail()
    {
        // Arrange
        var jobId = Guid.NewGuid();
        var command = new GetJobByIdQuery(jobId);
        
        var job = JobApplication.Create(_userContext.UserId, new CompanyName("Company A"), new Position("Dev"), new JobUrl("https://a.com"));
        job.AddNote("Some note");

        _repository.GetByIdAsync(jobId, Arg.Any<CancellationToken>()).Returns(job);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None).AsTask();

        // Assert
        Assert.That(result, Is.Not.Null);
        Assert.That(result.Id, Is.EqualTo(job.Id));
        Assert.That(result.CompanyName, Is.EqualTo("Company A"));
        Assert.That(result.Position, Is.EqualTo("Dev"));
        Assert.That(result.JobUrl, Is.EqualTo("https://a.com"));
        Assert.That(result.Notes.Count, Is.EqualTo(1));
        Assert.That(result.Notes.First().Content, Is.EqualTo("Some note"));
    }

    [Test]
    public void Handle_JobNotFound_ThrowsNotFoundException()
    {
        // Arrange
        var command = new GetJobByIdQuery(Guid.NewGuid());
        _repository.GetByIdAsync(command.JobId, Arg.Any<CancellationToken>()).ReturnsNull();

        // Act & Assert
        Assert.ThrowsAsync<NotFoundException>(() => _handler.Handle(command, CancellationToken.None).AsTask());
    }

    [Test]
    public void Handle_JobBelongsToDifferentUser_ThrowsNotFoundException()
    {
        // Arrange
        var jobId = Guid.NewGuid();
        var command = new GetJobByIdQuery(jobId);
        
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
