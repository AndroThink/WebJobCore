namespace AndroThink.WebJob.Core.Options;

/// <summary>
/// 
/// </summary>
public record JobLoggerOptions
{
    /// <summary>
    /// 
    /// </summary>
    public virtual string FileName { get; set; } = "";
    /// <summary>
    /// 
    /// </summary>
    public virtual string FolderPath { get; set; } = "";
}
