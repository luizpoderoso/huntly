using Huntly.Core.Jobs.Entities;

namespace Huntly.Core.Jobs.Repositories;

public interface IJobApplicationRepository
{
    Task<JobApplication?> GetByIdAsync(Guid id, CancellationToken ct);
    Task<IReadOnlyCollection<JobApplication>> GetAllByUserIdAsync(Guid userId, CancellationToken ct);
    Task AddAsync(JobApplication jobApplication, CancellationToken ct);
    void Remove(JobApplication jobApplication);
}