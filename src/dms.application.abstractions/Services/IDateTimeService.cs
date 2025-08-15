namespace DMS.Application.Abstractions.Services;

public interface IDateTimeService
{
    DateTime UtcNow { get; }
}