using DMS.Application.Abstractions.Outbox;
using DMS.Application.Abstractions.Persistence.Read;

namespace DMS.Web.BackgroundServices;

public class OutboxProcessor : BackgroundService
{
    private readonly IServiceScopeFactory _scopeFactory;
    private readonly TimeSpan _poll = TimeSpan.FromSeconds(1);
    private const int Batch = 100;

    private readonly ILogger<OutboxProcessor> _logger;

    public OutboxProcessor(ILoggerFactory factory, IServiceScopeFactory scopeFactory)
    {
        _scopeFactory = scopeFactory;
        _logger = factory.CreateLogger<OutboxProcessor>();
    }

    protected override async Task ExecuteAsync(CancellationToken ct)
    {
        var timer = new PeriodicTimer(_poll);
        while (await timer.WaitForNextTickAsync(ct))
        {
            using var scope = _scopeFactory.CreateScope();
            var outbox = scope.ServiceProvider.GetRequiredService<IOutbox>();
            //var projector = scope.ServiceProvider.GetRequiredService<IProjector>();

            var items = await outbox.GetPendingAsync(Batch, ct);
            if (items.Count == 0) continue;

            foreach (var m in items)
            {
                try
                {
                    //await projector.HandleAsync(m, ct);
                    await outbox.MarkProcessedAsync(m.Id, ct);
                }
                catch (Exception ex)
                {
                    var (attempts, nextTry) = await outbox.MarkFailedAsync(m.Id, ct);
                    _logger.LogWarning(ex, "Outbox {Id} failed. attempts={Attempts}, next={Next}", m.Id, attempts, nextTry);
                }
            }
        }
    }
}