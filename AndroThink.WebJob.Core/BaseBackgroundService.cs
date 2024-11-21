using Microsoft.Extensions.Hosting;

namespace AndroThink.WebJob.Core;

public abstract class BaseBackgroundService : IHostedService
{
    public BaseBackgroundService() { }

    public Task StartAsync(CancellationToken cancellationToken)
    {
        DoWork(cancellationToken).GetAwaiter().GetResult();
        return Task.CompletedTask;
    }

    public Task StopAsync(CancellationToken cancellationToken)
    {
        Console.WriteLine($"ProcessTasksService : stopping webjob .");
        return Task.CompletedTask;
    }

    private async Task DoWork(CancellationToken stoppingToken)
    {
        try
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                await ProcessTasksServiceAsync(stoppingToken);
                Environment.Exit(0);
            }
        }
        catch (Exception ex)
        {
            try
            {
                Console.WriteLine("Webjob Failed !" + ex.Message);
            }
            catch { }
            Environment.Exit(1);
        }
    }

    protected abstract Task ProcessTasksServiceAsync(CancellationToken stoppingToken);
}