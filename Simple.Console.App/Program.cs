using Microsoft.Extensions.Logging;
using Serilog;
using Serilog.Core;
using Serilog.Sinks.Datadog.Logs;
using System.Diagnostics;

namespace Simple.Console.App
{
    internal class Program
    {
        /// <summary>
        /// For more on this demo:
        /// https://github.com/DataDog/serilog-sinks-datadog-logs
        /// </summary>
        private static void Main()
        {
            DatadogConfiguration config =
                new(url: "intake.logs.datadoghq.com", port: 10516, useSSL: true, useTCP: true);

            using Logger log = new LoggerConfiguration()
                .WriteTo.DatadogLogs(
                    "",
                    source: "mx-test-src",
                    service: "mx-test-console-svc",
                    host: "mx-local-host",
                    tags: new string[] { "app:simple-console-app" },
                    configuration: config
                )
                .CreateLogger();

            Stopwatch sw = new();
            sw.Start();

            var user = new { Name = "Mohammed", Email = "test@email.com" };

            //log.Information("INF: Processed user: {@Position} in {Elapsed:000} ms.", user, sw.ElapsedMilliseconds);
            //log.Information("INF: Processed user: {Position}", user);
            //log.Warning("WRN: The system is performing poorly.");
            //log.Error("ERR: The system encountered an error.");



            using ILoggerFactory loggerFactory = new LoggerFactory();
            _ = loggerFactory.AddSerilog(log);
            Microsoft.Extensions.Logging.ILogger logger = loggerFactory.CreateLogger("Logger");

            using (logger.BeginScope(">>> THIS IS MY SCOPE <<<"))
            {
                logger.LogInformation("INF: Processed user: {@Position} in {Elapsed:000} ms.", user, sw.ElapsedMilliseconds);
                logger.LogInformation("INF: Processed user: {Position}", user);
                logger.LogWarning("WRN: The system is performing poorly.");
                logger.LogError("ERR: The system encountered an error.");
            }
        }
    }
}