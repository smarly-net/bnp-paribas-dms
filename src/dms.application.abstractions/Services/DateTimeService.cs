namespace DMS.Application.Abstractions.Services;

internal class DateTimeService : IDateTimeService
{
    public DateTime UtcNow => DateTime.UtcNow;
}