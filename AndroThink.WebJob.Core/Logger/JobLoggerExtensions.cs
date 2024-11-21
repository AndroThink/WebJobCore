using Microsoft.Extensions.Logging;
using Microsoft.Extensions.DependencyInjection;

namespace AndroThink.WebJob.Core.Logger;

/// <summary>
/// 
/// </summary>
public static class JobLoggerExtensions
{
    /// <summary>
    /// 
    /// </summary>
    /// <param name="builder"></param>
    /// <param name="configure"></param>
    /// <returns></returns>
    public static ILoggingBuilder AddJobLogger(this ILoggingBuilder builder, Action<Options.JobLoggerOptions> configure)
    {
        builder.Services.AddSingleton<ILoggerProvider, JobLoggerProvider>();
        builder.Services.Configure(configure);

        return builder;
    }
}