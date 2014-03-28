namespace RestApp.Core.Domain.Logging
{
    /// <summary>
    /// Represents a log level
    /// </summary>
    public enum LogLevel
    {
        Low = 1,
        Middle = 2,
        High = 3,
        None = 4,
        Emergency = 10,
        Debug = 100,
        Information = 200,
        Warning = 300,
        Error = 400,
        Fatal = 500

    }
}
