namespace DMS.Application.Abstractions.Notifications;

public interface INotificationSender
{
    Task SendAsync(Guid userId, string subject, string body, CancellationToken ct);
}