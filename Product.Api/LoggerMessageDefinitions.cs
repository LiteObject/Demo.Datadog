namespace Product.Api
{
    public static class LoggerMessageDefinitions
    {
        private static readonly Action<ILogger, string, Exception?> LogDbgDef =
            LoggerMessage.Define<string>(LogLevel.Debug, new EventId(1, "DEBUG"), "DBG: {Message}");

        private static readonly Action<ILogger, string, Exception?> LogInfDef =
            LoggerMessage.Define<string>(LogLevel.Information, new EventId(2, "INFORMATION"), "INF: {Message}");

        private static readonly Action<ILogger, string, Exception?> LogWrnDef =
            LoggerMessage.Define<string>(LogLevel.Warning, new EventId(3, "WARNING"), "WRN: {Message}");

        private static readonly Action<ILogger, string, Exception?> LogErrDef =
            LoggerMessage.Define<string>(LogLevel.Error, new EventId(4, "ERROR"), "ERR: {Message}");


        public static void LogErr(this ILogger logger, string message, Exception? exception)
        {
            LogErrDef(logger, message, exception);
        }

        public static void LogWrn(this ILogger logger, string message)
        {
            //if (logger.IsEnabled(LogLevel.Warning))
            //{
            //}

            LogWrnDef(logger, message, null);
        }

        public static void LogInf(this ILogger logger, string message)
        {
            LogInfDef(logger, message, null);
        }

        public static void LogDbg(this ILogger logger, string message)
        {
            LogDbgDef(logger, message, null);
        }
    }
}
