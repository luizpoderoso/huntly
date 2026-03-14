namespace Huntly.Application.Shared.Interfaces;

public interface IAtomicWork
{
    Task CommitAsync(CancellationToken ct);
}