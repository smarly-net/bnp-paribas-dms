namespace DMS.Application.Abstractions.Persistence.Write;

public interface IUnitOfWork
{
    Task Commit(CancellationToken ct);
}