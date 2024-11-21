using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace AndroThink.WebJob.Core;

public abstract class BaseHostedService : IHostedService
{
    protected readonly ILogger<BaseHostedService> Logger;

    public BaseHostedService(ILogger<BaseHostedService> logger)
    {
        Logger = logger;
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    public async Task StartAsync(CancellationToken cancellationToken)
    {
        while (!cancellationToken.IsCancellationRequested)
        {
            Logger.LogInformation($"Job running at: {DateTimeOffset.Now.ToString("yyyy-MM-dd hh:mm:ss tt")}");
            await ProcessServiceTaskAsync(cancellationToken);
            Logger.LogInformation($"Job finished at: {DateTimeOffset.Now.ToString("yyyy-MM-dd hh:mm:ss tt")}");

            await StopAsync(cancellationToken);
        }

        Logger.LogInformation($"Job cancelled at: {DateTimeOffset.Now.ToString("yyyy-MM-dd hh:mm:ss tt")}");
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
    public async Task StopAsync(CancellationToken cancellationToken)
    {
        Logger.LogInformation("Job stopping gracefully...");
        Environment.Exit(0);
    }
}