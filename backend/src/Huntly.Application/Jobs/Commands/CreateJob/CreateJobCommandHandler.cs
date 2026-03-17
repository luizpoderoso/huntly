using Huntly.Application.Shared.DTOs.Jobs;
using Huntly.Application.Shared.Interfaces;
using Huntly.Core.Jobs.Entities;
using Huntly.Core.Jobs.Repositories;
using Huntly.Core.Jobs.ValueObjects;
using MediatR;

namespace Huntly.Application.Jobs.Commands.CreateJob;

public class CreateJobCommandHandler(
    IJobApplicationRepository repository,
    IAtomicWork atomicWork,
    IUserContext userContext)
    : IRequestHandler<CreateJobCommand, JobSummaryDto>
{
    public async Task<JobSummaryDto> Handle(CreateJobCommand command, CancellationToken ct)
    {
        var job = JobApplication.Create(
            userId: userContext.UserId,
            companyName: new CompanyName(command.CompanyName),
            position: new Position(command.Position),
            jobUrl: command.JobUrl is not null ? new JobUrl(command.JobUrl) : null,
            salaryRange: command.SalaryMin.HasValue
                ? new SalaryRange(command.SalaryMin.Value, command.SalaryMax!.Value, command.SalaryCurrency ?? "USD")
                : null
        );

        await repository.AddAsync(job, ct);
        await atomicWork.CommitAsync(ct);

        return new JobSummaryDto(
            job.Id, 
            job.CompanyName.Value, 
            job.Position.Value, 
            job.Status.ToString(),
            job.JobUrl?.Value,
            job.CreatedAt, 
            job.UpdatedAt);
    }
}