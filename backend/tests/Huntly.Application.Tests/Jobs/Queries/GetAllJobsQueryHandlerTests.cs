using Huntly.Application.Jobs.Queries.GetAllJobs;
using Huntly.Application.Shared.DTOs.Jobs;
using Huntly.Application.Shared.Interfaces;
using Huntly.Core.Jobs.Entities;
using Huntly.Core.Jobs.Repositories;
using Huntly.Core.Jobs.ValueObjects;
using NSubstitute;
using NUnit.Framework;

namespace Huntly.Application.Tests.Jobs.Queries;

[TestFixture]
public class GetAllJobsQueryHandlerTests
{
    private IJobApplicationRepository _repository;
    private IUserContext _userContext;
    private GetAllJobsQueryHandler _handler;

    [SetUp]
    public void SetUp()
    {
        _repository = Substitute.For<IJobApplicationRepository>();
        _userContext = Substitute.For<IUserContext>();

        _userContext.UserId.Returns(Guid.Parse("55555555-5555-5555-5555-555555555555"));

        _handler = new GetAllJobsQueryHandler(_repository, _userContext);
    }

    [Test]
    public async Task Handle_ValidQuery_ReturnsMappedJobSummaries()
    {
        // Arrange
        var command = new GetAllJobsQuery();
        
        var job1 = JobApplication.Create(_userContext.UserId, new CompanyName("Company A"), new Position("Dev"), new JobUrl("https://a.com"));
        var job2 = JobApplication.Create(_userContext.UserId, new CompanyName("Company B"), new Position("QA"));

        var jobList = new List<JobApplication> { job1, job2 };

        _repository.GetAllByUserIdAsync(_userContext.UserId, Arg.Any<CancellationToken>()).Returns(jobList);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None).AsTask();

        // Assert
        Assert.That(result, Is.Not.Null);
        Assert.That(result.Count, Is.EqualTo(2));
        
        var firstResult = result.First(j => j.Id == job1.Id);
        Assert.That(firstResult.CompanyName, Is.EqualTo("Company A"));
        Assert.That(firstResult.Position, Is.EqualTo("Dev"));
        Assert.That(firstResult.JobUrl, Is.EqualTo("https://a.com"));

        var secondResult = result.First(j => j.Id == job2.Id);
        Assert.That(secondResult.CompanyName, Is.EqualTo("Company B"));
        Assert.That(secondResult.JobUrl, Is.Null);
    }

    [Test]
    public async Task Handle_NoJobsFound_ReturnsEmptyCollection()
    {
        // Arrange
        var command = new GetAllJobsQuery();
        _repository.GetAllByUserIdAsync(_userContext.UserId, Arg.Any<CancellationToken>()).Returns(new List<JobApplication>());

        // Act
        var result = await _handler.Handle(command, CancellationToken.None).AsTask();

        // Assert
        Assert.That(result, Is.Not.Null);
        Assert.That(result, Is.Empty);
    }
}
