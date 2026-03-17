using Huntly.Application.Jobs.Commands.CreateJob;
using Huntly.Application.Shared.DTOs.Jobs;
using Huntly.Application.Shared.Interfaces;
using Huntly.Core.Jobs.Entities;
using Huntly.Core.Jobs.Repositories;
using Huntly.Core.Jobs.ValueObjects;
using NSubstitute;
using NUnit.Framework;

namespace Huntly.Application.Tests.Jobs.Commands;

[TestFixture]
public class CreateJobCommandHandlerTests
{
    private IJobApplicationRepository _repository;
    private IAtomicWork _atomicWork;
    private IUserContext _userContext;
    private CreateJobCommandHandler _handler;

    [SetUp]
    public void SetUp()
    {
        _repository = Substitute.For<IJobApplicationRepository>();
        _atomicWork = Substitute.For<IAtomicWork>();
        _userContext = Substitute.For<IUserContext>();
        
        _userContext.UserId.Returns(Guid.Parse("00000000-0000-0000-0000-000000000001"));

        _handler = new CreateJobCommandHandler(_repository, _atomicWork, _userContext);
    }

    [Test]
    public async Task Handle_ValidCommand_CreatesJobAndCallsRepository()
    {
        // Arrange
        var command = new CreateJobCommand(
            "Test Company",
            "Software Engineer",
            "https://example.com/job",
            50000,
            80000,
            "USD"
        );

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        // Verify repository Add method was called with correctly mapped properties
        var capturedJob = (JobApplication) _repository.ReceivedCalls()
            .Single(c => c.GetMethodInfo().Name == "AddAsync")
            .GetArguments()[0]!;

        Assert.That(capturedJob.UserId.ToString(), Is.EqualTo("00000000-0000-0000-0000-000000000001"));
        Assert.That(capturedJob.CompanyName.Value, Is.EqualTo("Test Company"));
        Assert.That(capturedJob.Position.Value, Is.EqualTo("Software Engineer"));
        Assert.That(capturedJob.JobUrl?.Value, Is.EqualTo("https://example.com/job"));
        Assert.That(capturedJob.SalaryRange?.Min, Is.EqualTo(50000));
        Assert.That(capturedJob.SalaryRange?.Max, Is.EqualTo(80000));
        Assert.That(capturedJob.SalaryRange?.Currency, Is.EqualTo("USD"));

        // Verify Commit was called
        await _atomicWork.Received(1).CommitAsync(Arg.Any<CancellationToken>());

        // Verify Returns
        Assert.That(result, Is.Not.Null);
        Assert.That(result.CompanyName, Is.EqualTo("Test Company"));
        Assert.That(result.Position, Is.EqualTo("Software Engineer"));
        Assert.That(result.JobUrl, Is.EqualTo("https://example.com/job"));
        Assert.That(result.Status, Is.EqualTo(Huntly.Core.Jobs.Enums.ApplicationStatus.Applied.ToString())); // Default
    }

    [Test]
    public async Task Handle_CommandWithoutOptionalFields_CreatesJobSuccesfully()
    {
        // Arrange
        var command = new CreateJobCommand(
            "Test Company",
            "Software Engineer",
            null,
            null,
            null,
            null
        );

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        var capturedJob = (JobApplication) _repository.ReceivedCalls()
            .Single(c => c.GetMethodInfo().Name == "AddAsync")
            .GetArguments()[0]!;

        Assert.That(capturedJob.UserId.ToString(), Is.EqualTo("00000000-0000-0000-0000-000000000001"));
        Assert.That(capturedJob.CompanyName.Value, Is.EqualTo("Test Company"));
        Assert.That(capturedJob.Position.Value, Is.EqualTo("Software Engineer"));
        Assert.That(capturedJob.JobUrl, Is.Null);
        Assert.That(capturedJob.SalaryRange, Is.Null);

        await _atomicWork.Received(1).CommitAsync(Arg.Any<CancellationToken>());

        Assert.That(result, Is.Not.Null);
        Assert.That(result.JobUrl, Is.Null);
    }
}
