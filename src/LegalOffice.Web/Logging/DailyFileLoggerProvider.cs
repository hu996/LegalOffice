using Microsoft.Extensions.Logging;
using System.Text;

namespace LegalOffice.Web.Logging;

public sealed class DailyFileLoggerProvider : ILoggerProvider
{
    private readonly string _logsDirectory;
    private readonly LogLevel _minimumLevel;
    private readonly object _fileLock = new();

    public DailyFileLoggerProvider(string logsDirectory, LogLevel minimumLevel = LogLevel.Error)
    {
        _logsDirectory = logsDirectory;
        _minimumLevel = minimumLevel;
        Directory.CreateDirectory(_logsDirectory);
    }

    public ILogger CreateLogger(string categoryName)
    {
        return new DailyFileLogger(categoryName, _logsDirectory, _minimumLevel, _fileLock);
    }

    public void Dispose()
    {
    }

    private sealed class DailyFileLogger : ILogger
    {
        private readonly string _categoryName;
        private readonly string _logsDirectory;
        private readonly LogLevel _minimumLevel;
        private readonly object _fileLock;

        public DailyFileLogger(string categoryName, string logsDirectory, LogLevel minimumLevel, object fileLock)
        {
            _categoryName = categoryName;
            _logsDirectory = logsDirectory;
            _minimumLevel = minimumLevel;
            _fileLock = fileLock;
        }

        public IDisposable BeginScope<TState>(TState state) where TState : notnull
        {
            return NullScope.Instance;
        }

        public bool IsEnabled(LogLevel logLevel)
        {
            return logLevel >= _minimumLevel && logLevel != LogLevel.None;
        }

        public void Log<TState>(
            LogLevel logLevel,
            EventId eventId,
            TState state,
            Exception? exception,
            Func<TState, Exception?, string> formatter)
        {
            if (!IsEnabled(logLevel))
            {
                return;
            }

            var message = formatter(state, exception);
            if (string.IsNullOrWhiteSpace(message) && exception is null)
            {
                return;
            }

            var now = DateTime.Now;
            var filePath = Path.Combine(_logsDirectory, $"errors-{now:yyyy-MM-dd}.log");
            var entry = new StringBuilder()
                .Append(now.ToString("yyyy-MM-dd HH:mm:ss.fff"))
                .Append(" [")
                .Append(logLevel)
                .Append("] ")
                .Append(_categoryName)
                .Append(" (")
                .Append(eventId.Id)
                .Append(") ")
                .AppendLine(message);

            if (exception is not null)
            {
                entry.AppendLine("Exception details:");
                AppendExceptionDetails(entry, exception, 0);
            }

            lock (_fileLock)
            {
                File.AppendAllText(filePath, entry.ToString() + Environment.NewLine);
            }
        }

        private static void AppendExceptionDetails(StringBuilder builder, Exception exception, int depth)
        {
            var indent = new string(' ', depth * 2);

            builder.Append(indent)
                .Append(exception.GetType().FullName)
                .Append(": ")
                .AppendLine(exception.Message);

            if (!string.IsNullOrWhiteSpace(exception.StackTrace))
            {
                builder.Append(indent)
                    .AppendLine(exception.StackTrace);
            }

            if (exception.InnerException is not null)
            {
                builder.Append(indent)
                    .AppendLine("Inner exception:");
                AppendExceptionDetails(builder, exception.InnerException, depth + 1);
            }
        }

        private sealed class NullScope : IDisposable
        {
            public static readonly NullScope Instance = new();

            public void Dispose()
            {
            }
        }
    }
}
