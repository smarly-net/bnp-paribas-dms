using DMS.Application.Abstractions.Notifications;

using Microsoft.Extensions.Logging;

namespace DMS.Infrastructure.Notifications;

public sealed class EmailNotificationSender : INotificationSender
{
    private readonly ILogger<EmailNotificationSender> _logger;
    public EmailNotificationSender(ILogger<EmailNotificationSender> logger) => _logger = logger;

    public Task SendAsync(Guid userId, string subject, string body, CancellationToken ct)
    {
        _logger.LogInformation("Notify {UserId}: {Subject} | {Body}", userId, subject, body);
        return Task.CompletedTask;
    }
}