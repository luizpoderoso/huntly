using Huntly.Application.Seed;
using Huntly.Application.Shared.Exceptions;
using Huntly.Application.Shared.Interfaces;
using Huntly.Core.Jobs.Entities;
using Huntly.Core.Jobs.Repositories;
using NSubstitute;
using NUnit.Framework;

namespace Huntly.Application.Tests.Seed;

[TestFixture]
public class SeedCommandHandlerTests
{
    private IJobApplicationRepository _repository;
    private IAtomicWork _atomicWork;
    private IUserContext _userContext;
    private SeedCommandHandler _handler;

    [SetUp]
    public void SetUp()
    {
        _repository = Substitute.For<IJobApplicationRepository>();
        _atomicWork = Substitute.For<IAtomicWork>();
        _userContext = Substitute.For<IUserContext>();

        _userContext.UserId.Returns(Guid.Parse("99999999-9999-9999-9999-999999999999"));

        _handler = new SeedCommandHandler(_repository, _atomicWork, _userContext);
    }

    [Test]
    public async Task Handle_UserHasNoData_SeedsSuccessfully()
    {
        var command = new SeedCommand();
        _repository.CountByUserIdAsync(_userContext.UserId, Arg.Any<CancellationToken>()).Returns(0);

        await _handler.Handle(command, CancellationToken.None);

        await _repository.Received(10).AddAsync(Arg.Any<JobApplication>(), Arg.Any<CancellationToken>());
        await _atomicWork.Received(1).CommitAsync(Arg.Any<CancellationToken>());
    }

    [Test]
    public async Task Handle_UserHasLessThan5Jobs_SeedsSuccessfully()
    {
        var command = new SeedCommand();
        _repository.CountByUserIdAsync(_userContext.UserId, Arg.Any<CancellationToken>()).Returns(4);

        await _handler.Handle(command, CancellationToken.None);

        await _repository.Received(10).AddAsync(Arg.Any<JobApplication>(), Arg.Any<CancellationToken>());
        await _atomicWork.Received(1).CommitAsync(Arg.Any<CancellationToken>());
    }

    [Test]
    public void Handle_UserHas5OrMoreJobs_ThrowsConflictException()
    {
        var command = new SeedCommand();
        _repository.CountByUserIdAsync(_userContext.UserId, Arg.Any<CancellationToken>()).Returns(5);

        var ex = Assert.ThrowsAsync<ConflictException>(() => _handler.Handle(command, CancellationToken.None));
        Assert.That(ex.Message, Is.EqualTo("You already have data. Clear your applications before seeding."));
    }
}
