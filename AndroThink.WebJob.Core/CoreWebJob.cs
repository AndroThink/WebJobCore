using Microsoft.Azure.WebJobs;
using AndroThink.WebJob.Core.Logger;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace AndroThink.WebJob.Core;

/// <summary>
/// 
/// </summary>
public class CoreWebJob
{
    private static CoreWebJob? eliteWebJob = null;

    private HostBuilder builder;
    private bool isLoggerRegistered;
    private ILogger<CoreWebJob>? _logger;
    private readonly IConfiguration? _configuration;

    /// <summary>
    /// Get or create the webjob instance
    /// </summary>
    /// <param name="appSettingFileName"></param>
    /// <param name="useEnvironmentVariables"></param>
    /// <returns></returns>
    public static CoreWebJob Create(string appSettingFileName = "appsettings.json", bool useEnvironmentVariables = true)
    {
        if (eliteWebJob is null)
            eliteWebJob = new CoreWebJob(appSettingFileName, useEnvironmentVariables);

        return eliteWebJob;
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="appSettingFileName"></param>
    /// <param name="useEnvironmentVariables"></param>
    private CoreWebJob(string appSettingFileName, bool useEnvironmentVariables)
    {
        builder = new HostBuilder();

        builder.ConfigureAppConfiguration((context, appBuilder) =>
        {
            if (!string.IsNullOrEmpty(appSettingFileName))
                appBuilder.AddJsonFile(appSettingFileName);

            if (useEnvironmentVariables)
                appBuilder.AddEnvironmentVariables();
        });
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="configure"></param>
    /// <returns></returns>
    public CoreWebJob ConfigureWebJobs(Action<IWebJobsBuilder> configure)
    {
        builder.ConfigureWebJobs(configure, delegate { });

        return this;
    }

    /// <summary>
    ///  Adds a delegate for configuring the provided Microsoft.Extensions.Logging.ILoggingBuilder.
    ///  This may be called multiple times.
    /// </summary>
    /// <param name="configureLogging"></param>
    /// <returns></returns>
    public CoreWebJob ConfigureLogging(Action<HostBuilderContext, ILoggingBuilder> configureLogging)
    {
        builder.ConfigureLogging(configureLogging);
        return this;
    }

    /// <summary>
    /// Add webjob logger that logs in a file
    /// </summary>
    /// <param name="folderPath"></param>
    /// <param name="webJobName"></param>
    /// <returns></returns>
    public CoreWebJob WithFileLogger(string folderPath = "Logs", string webJobName = "WebJob")
    {
        var logFolder = Path.Combine(Environment.GetEnvironmentVariable("WEBROOT_PATH") ?? "", string.IsNullOrEmpty(folderPath) ? @"Logs" : folderPath);

        builder.ConfigureLogging(c =>
        {
            c.AddJobLogger(op =>
            {
                op.FolderPath = logFolder;
                op.FileName = $"{webJobName}-{{date}}.txt";
            });
        });

        isLoggerRegistered = true;

        return this;
    }

    /// <summary>
    /// Adds services to the container. This can be called multiple times and the results will be additive.
    /// </summary>
    /// <param name="configureDelegate"></param>
    /// <returns></returns>
    public CoreWebJob ConfigureServices(Action<HostBuilderContext, IServiceCollection> configureDelegate)
    {
        builder.ConfigureServices(configureDelegate);

        return this;
    }

    /// <summary>
    ///  Listens for Ctrl+C or SIGTERM and calls Microsoft.Extensions.Hosting.IApplicationLifetime.StopApplication
    ///  to start the shutdown process. This will unblock extensions like RunAsync and
    ///  WaitForShutdownAsync.
    /// </summary>
    /// <returns></returns>
    public CoreWebJob UseConsoleLifetime()
    {
        builder.UseConsoleLifetime();
        return this;
    }

    /// <summary>
    /// run the web job
    /// </summary>
    /// <param name="unhandledExceptionEventHandler"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    public async Task RunAsync(UnhandledExceptionEventHandler? unhandledExceptionEventHandler, CancellationToken cancellationToken = default)
    {

        if (!isLoggerRegistered)
        {
            if (unhandledExceptionEventHandler != null)
            {
                AppDomain currentDomain = AppDomain.CurrentDomain;
                currentDomain.UnhandledException += unhandledExceptionEventHandler;
            }

            await builder.Build().RunAsync(cancellationToken);
        }
        else
        {
            using (var host = builder.Build())
            {
                try
                {
                    _logger = host.Services.GetRequiredService<ILogger<CoreWebJob>>();

                    _logger?.LogInformation("WebJob Successfully Started ... waitting for queues to execute ...");
                }
                catch (Exception ex)
                {
                    Console.WriteLine("WebJob Esxception ==> " + ex.ToString());
                }

                AppDomain currentDomain = AppDomain.CurrentDomain;
                currentDomain.UnhandledException += (sender, args) =>
                {
                    if (unhandledExceptionEventHandler != null)
                        unhandledExceptionEventHandler(this, args);

                    Exception ex = (Exception)args.ExceptionObject;

                    if (_logger != null)
                        _logger.LogCritical(ex, ex.Message);
                    else
                        Console.WriteLine("WebJob Esxception ==> " + ex.ToString());
                };

                _logger?.LogInformation("WebJob Successfully Started ... waitting for queues to execute ...");

                await host.RunAsync(cancellationToken);
            }
        }
    }
}