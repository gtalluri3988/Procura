using BusinessLogic.Interfaces;

namespace Api.Workers
{
    public class EmailOutboxWorker : BackgroundService
    {
        private static readonly TimeSpan PollInterval = TimeSpan.FromSeconds(60);
        private const int BatchSize = 20;

        private readonly IServiceProvider _services;
        private readonly ILogger<EmailOutboxWorker> _logger;

        public EmailOutboxWorker(IServiceProvider services, ILogger<EmailOutboxWorker> logger)
        {
            _services = services;
            _logger = logger;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _logger.LogInformation("EmailOutboxWorker started. Poll interval: {Interval}s", PollInterval.TotalSeconds);

            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    using var scope = _services.CreateScope();
                    var emailService = scope.ServiceProvider.GetRequiredService<IEmailService>();
                    var processed = await emailService.ProcessQueueAsync(BatchSize, stoppingToken);
                    if (processed > 0)
                    {
                        _logger.LogInformation("EmailOutboxWorker processed {Count} queued messages.", processed);
                    }
                }
                catch (OperationCanceledException)
                {
                    // shutdown — fall through
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "EmailOutboxWorker poll iteration failed.");
                }

                try
                {
                    await Task.Delay(PollInterval, stoppingToken);
                }
                catch (OperationCanceledException)
                {
                    break;
                }
            }

            _logger.LogInformation("EmailOutboxWorker stopping.");
        }
    }
}
