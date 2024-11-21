using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace AndroThink.WebJob.Core;

public abstract class BaseBackgroundService : BackgroundService
{
    private readonly ILogger<BaseBackgroundService> _logger;

    public BaseBackgroundService(ILogger<BaseBackgroundService> logger)
    {
        _logger = logger;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            _logger.LogInformation("Worker running at: {time}", DateTimeOffset.Now);
            await ProcessServiceTaskAsync(stoppingToken);
        }

        _logger.LogInformation("Worker stopping...");
    }

    protected abstract Task ProcessServiceTaskAsync(CancellationToken stoppingToken);

    public override async Task StopAsync(CancellationToken cancellationToken)
    {
        _logger.LogInformation("Worker stopping gracefully...");
        await base.StopAsync(cancellationToken);
    }
}