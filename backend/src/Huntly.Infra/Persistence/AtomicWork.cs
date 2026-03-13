using Huntly.Application.Shared.Interfaces;
using Huntly.Infra.Persistence.Context;

namespace Huntly.Infra.Persistence;

public class AtomicWork(AppDbContext context) : IAtomicWork
{
    public async Task CommitAsync(CancellationToken ct) => await context.SaveChangesAsync(ct);
}