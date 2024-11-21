using Microsoft.Extensions.Logging;
using System.Diagnostics.CodeAnalysis;
using System.Diagnostics;
using AndroThink.WebJob.Core.Utils;

namespace AndroThink.WebJob.Core.Logger;

/// <summary>
/// 
/// </summary>
public class JobLogger : ILogger
{
    private static readonly object lockObject = new object();

    protected readonly JobLoggerProvider _jobLoggerProvider;

    public JobLogger([NotNull] JobLoggerProvider jobLoggerProvider)
    {
        _jobLoggerProvider = jobLoggerProvider;
    }

    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="TState"></typeparam>
    /// <param name="state"></param>
    /// <returns></returns>
    public IDisposable? BeginScope<TState>(TState state) where TState : notnull
    {
        return null;
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="logLevel"></param>
    /// <returns></returns>
    public bool IsEnabled(LogLevel logLevel)
    {
        return logLevel != LogLevel.None;
    }

    /// <summary>
    /// Log 
    /// </summary>
    /// <typeparam name="TState"></typeparam>
    /// <param name="logLevel"></param>
    /// <param name="eventId"></param>
    /// <param name="state"></param>
    /// <param name="exception"></param>
    /// <param name="formatter"></param>
    public void Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception? exception, Func<TState, Exception?, string> formatter)
    {
        if (!IsEnabled(logLevel) || exception is not null && exception.ToString().ToLower().Contains("comexception"))
        {
            return;
        }

        string fullFilePath = $"{_jobLoggerProvider.Options.FolderPath}\\{_jobLoggerProvider.Options.FileName.Replace("{date}", DateTimeOffset.UtcNow.ToString("yyyyMMdd"))}";

        string exceptionDetails = ExceptionUtils.GetExceptionDetails(exception);

        string logRecord = $"{"[" + DateTimeOffset.UtcNow.ToString("yyyy-MM-dd HH:mm:ss+00:00", new System.Globalization.CultureInfo("en-US")) + "]"} " +
                           $"[{logLevel}] {formatter(state, exception)} {(exception is not null ? exceptionDetails : "")}{Environment.NewLine}---------------------------------------------------{Environment.NewLine}{Environment.NewLine}";

        WriteLogToFile(fullFilePath, logRecord);
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="filePath"></param>
    /// <param name="logRecord"></param>
    /// <returns></returns>
    private bool WriteLogToFile(string filePath, string logRecord)
    {
        try
        {
            lock (lockObject)
            {
                using (StreamWriter streamWriter = new StreamWriter(filePath, true))
                {
                    streamWriter.WriteLine(logRecord);
                }

                return true;
            }
        }
        catch (Exception ex)
        {
            Debug.WriteLine(ex);
            return false;
        }
    }
}