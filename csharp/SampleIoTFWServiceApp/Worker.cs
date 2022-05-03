using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace SampleIoTFWServiceApp
{
    public class Worker : BackgroundService
    {
        private readonly ILogger<Worker> _logger;

        public Worker(ILogger<Worker> logger)
        {
            _logger = logger;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            var iotApp = new IoTApp(new MELLogger(_logger));

            await iotApp.UserPreInitializeAsync();
            _logger.LogInformation("UserPreInitializedAsync has been done at: {time}", DateTimeOffset.Now);
            await iotApp.InitializeAsync("iot-app-config.yaml");
            _logger.LogInformation("InitializedAsync has been done at: {time}", DateTimeOffset.Now);
            await iotApp.UserPostInitializeAsync();
            _logger.LogInformation("UserPostInitializedAsync has been done at: {time}", DateTimeOffset.Now);

            await iotApp.DoWorkAsync();
            _logger.LogInformation("DoWorkAsync has been called at: {time}", DateTimeOffset.Now);

            await WhenCancelled(stoppingToken);

            await iotApp.UserPreTerminateAsync();
            _logger.LogInformation("UserPreTerminatedAsync has been done at: {time}", DateTimeOffset.Now);
            await iotApp.TerminateAsync();
            _logger.LogInformation("TerminatedAsync has been done at: {time}", DateTimeOffset.Now);
            await iotApp.UserPostTerminateAsync();
            _logger.LogInformation("UserPostTerminatedAsync has been done at: {time}", DateTimeOffset.Now);
        }
        public static Task WhenCancelled(CancellationToken cancellationToken)
        {
            var tcs = new TaskCompletionSource<bool>();
            cancellationToken.Register(s => ((TaskCompletionSource<bool>)s).SetResult(true), tcs);
            return tcs.Task;
        }
    }

    class MELLogger : Kae.Utility.Logging.Logger
    {
        ILogger logger;
        public MELLogger(ILogger<Worker> logger)
        {
            this.logger = logger;
        }
        protected override async Task LogInternal(Level level, string log, string timestamp)
        {
            switch (level)
            {
                case Level.Info:
                    logger.LogInformation($"{timestamp} {log}");
                    break;
                case Level.Warn:
                    logger.LogWarning($"{timestamp} {log}");
                    break;
                case Level.Erro:
                    logger.LogError($"{timestamp} {log}");
                    break;
            }
        }
    }
}
