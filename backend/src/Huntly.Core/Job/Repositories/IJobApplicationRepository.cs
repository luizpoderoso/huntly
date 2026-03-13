using Huntly.Core.Job.Entities;

namespace Huntly.Core.Job.Repositories;

public interface IJobApplicationRepository
{
    Task<JobApplication?> GetByIdAsync(Guid id, CancellationToken ct);
    Task<IReadOnlyCollection<JobApplication>> GetAllByUserIdAsync(Guid userId, CancellationToken ct);
    Task AddAsync(JobApplication jobApplication, CancellationToken ct);
    void Remove(JobApplication jobApplication);
}