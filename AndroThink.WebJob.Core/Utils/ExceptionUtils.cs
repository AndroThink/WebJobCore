namespace AndroThink.WebJob.Core.Utils;
/// <summary>
/// Exception utils
/// </summary>
public class ExceptionUtils
{
    /// <summary>
    /// Get exception details
    /// </summary>
    /// <param name="exception"></param>
    /// <returns></returns>
    public static string GetExceptionDetails(Exception? exception)
    {
        string exceptionDetails = "";
        var exc = exception;
        while (exc is not null)
        {
            exceptionDetails += exc.Message + " | " + exc.StackTrace + GetEntriesFromEFExceptions(exc);
            exc = exc.InnerException;
        }

        return exceptionDetails;
    }

    /// <summary>
    /// Get Entries From EF Exceptions
    /// </summary>
    /// <param name="exception"></param>
    /// <returns></returns>
    public static string GetEntriesFromEFExceptions(Exception exception)
    {
        string data = "";
        if (exception != null && exception is Microsoft.EntityFrameworkCore.DbUpdateException)
        {
            var efExc = (Microsoft.EntityFrameworkCore.DbUpdateException)exception;
            foreach (var entry in efExc.Entries)
            {
                data += GetEntryDetails(entry) + Environment.NewLine;
            }

            data = Environment.NewLine + "Entries => " + data;
        }

        return data;
    }

    /// <summary>
    /// Get Assembly File Version
    /// </summary>
    /// <returns></returns>
    public static string GetAssemblyFileVersion()
    {
        System.Reflection.Assembly assembly = System.Reflection.Assembly.GetExecutingAssembly();
        System.Diagnostics.FileVersionInfo fvi = System.Diagnostics.FileVersionInfo.GetVersionInfo(assembly.Location);
        return fvi.FileVersion ?? "";
    }

    /// <summary>
    /// Get Entry Details
    /// </summary>
    /// <param name="entry"></param>
    /// <returns></returns>
    private static string GetEntryDetails(Microsoft.EntityFrameworkCore.ChangeTracking.EntityEntry entry)
    {
        string details = $"Entry ({entry.GetType().FullName}) : ";

        var properties = entry.GetType().GetProperties();
        if (properties != null)
        {
            foreach (var prop in properties)
            {
                if (prop == null)
                    continue;

                details += $"{prop.Name}-{prop.GetValue(entry, null)?.ToString() ?? ""}" + Environment.NewLine;
            }
        }

        return details;
    }

}
