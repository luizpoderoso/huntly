using Huntly.Core.Job.Entities;
using Huntly.Core.Job.Repositories;
using Huntly.Infra.Persistence.Context;
using Microsoft.EntityFrameworkCore;

namespace Huntly.Infra.Persistence.Repositories.Jobs;

public class JobApplicationRepository(AppDbContext context) : IJobApplicationRepository
{
    public async Task<JobApplication?> GetByIdAsync(Guid id, CancellationToken ct)
    {
        return await context.JobApplications
            .Include(j => j.Interviews)
            .Include(j => j.Notes)
            .FirstOrDefaultAsync(j => j.Id == id, ct);
    }

    public async Task<IReadOnlyCollection<JobApplication>> GetAllByUserIdAsync(Guid userId, CancellationToken ct)
    {
        return await context.JobApplications
            .Where(j => j.UserId == userId)
            .ToListAsync(ct);
    }

    public async Task AddAsync(JobApplication jobApplication, CancellationToken ct)
    {
        await context.JobApplications.AddAsync(jobApplication, ct);
    }

    public void Remove(JobApplication jobApplication)
    {
        context.Remove(jobApplication);
    }
}