using AndroThink.WebJob.Core.Options;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace AndroThink.WebJob.Core.Logger;

/// <summary>
/// 
/// </summary>
[ProviderAlias("JobLoggerProvider")]
public class JobLoggerProvider : ILoggerProvider, IDisposable
{
    public readonly JobLoggerOptions Options;

    /// <summary>
    /// 
    /// </summary>
    /// <param name="options"></param>
    public JobLoggerProvider(IOptions<JobLoggerOptions> options)
    {
        Options = options.Value;

        if (!Directory.Exists(Options.FolderPath))
        {
            Directory.CreateDirectory(Options.FolderPath);
        }
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="options"></param>
    public JobLoggerProvider(JobLoggerOptions options)
    {
        Options = options;
        if (!Directory.Exists(Options.FolderPath))
        {
            Directory.CreateDirectory(Options.FolderPath);
        }
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="categoryName"></param>
    /// <returns></returns>
    public ILogger CreateLogger(string categoryName)
    {
        return new JobLogger(this);
    }

    /// <summary>
    /// 
    /// </summary>
    public void Dispose()
    {
    }
}
