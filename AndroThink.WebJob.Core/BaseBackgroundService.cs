using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace AndroThink.WebJob.Core;

public abstract class BaseBackgroundService : BackgroundService
{
    protected readonly ILogger<BaseBackgroundService> Logger;

    public BaseBackgroundService(ILogger<BaseBackgroundService> logger)
    {
        Logger = logger;
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="stoppingToken"></param>
    /// <returns></returns>
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            Logger.LogInformation($"Worker running at: {DateTimeOffset.Now.ToString("yyyy-MM-dd hh:mm:ss tt")}");
            await ProcessServiceTaskAsync(stoppingToken);
            Logger.LogInformation($"Worker finished at: {DateTimeOffset.Now.ToString("yyyy-MM-dd hh:mm:ss tt")}");

            await StopAsync(stoppingToken);
        }

        Logger.LogInformation($"Worker cancelled at: {DateTimeOffset.Now.ToString("yyyy-MM-dd hh:mm:ss tt")}");
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="stoppingToken"></param>
    /// <returns></returns>
    protected abstract Task ProcessServiceTaskAsync(CancellationToken stoppingToken);

    /// <summary>
    /// 
    /// </summary>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    public override async Task StopAsync(CancellationToken cancellationToken)
    {
        Logger.LogInformation("Worker stopping gracefully...");
        await base.StopAsync(cancellationToken);

        Environment.Exit(0);
    }
}