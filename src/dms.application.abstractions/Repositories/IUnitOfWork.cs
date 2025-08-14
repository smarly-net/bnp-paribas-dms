namespace DMS.Application.Abstractions.Repositories;

public interface IUnitOfWork
{
    Task Commit(CancellationToken ct);
}