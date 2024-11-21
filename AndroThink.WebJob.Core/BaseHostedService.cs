using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace AndroThink.WebJob.Core;

public abstract class BaseHostedService : IHostedService
{
    private Task? _executingTask;
    private readonly ILogger<BaseHostedService> _logger;
    private readonly CancellationTokenSource _stoppingCts = new CancellationTokenSource();

    public BaseHostedService(ILogger<BaseHostedService> logger)
    {
        _logger = logger;
    }

    public Task StartAsync(CancellationToken cancellationToken)
    {
        _logger.LogInformation("Job started.");
        _executingTask = ExecuteAsync(_stoppingCts.Token);
        return _executingTask.IsCompleted ? _executingTask : Task.CompletedTask;
    }

    public async Task StopAsync(CancellationToken cancellationToken)
    {
        _logger.LogInformation("MyHostedService stopping.");
        if (_executingTask == null) return;

        _stoppingCts.Cancel();

        await Task.WhenAny(_executingTask, Task.Delay(Timeout.Infinite, cancellationToken));
    }

    protected virtual async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            await ProcessServiceTaskAsync(stoppingToken);
        }
    }

    protected abstract Task ProcessServiceTaskAsync(CancellationToken stoppingToken);
}