using BusinessLogic.Interfaces;
using DB.Repositories.Interfaces;

namespace Api.Workers
{
    public class VendorReminderWorker : BackgroundService
    {
        private static readonly int[] Thresholds = { 90, 30, 14, 7, 3, 1 };
        private static readonly TimeSpan DailyRunTimeUtc = TimeSpan.FromHours(2); // 02:00 UTC

        private readonly IServiceProvider _services;
        private readonly ILogger<VendorReminderWorker> _logger;

        public VendorReminderWorker(IServiceProvider services, ILogger<VendorReminderWorker> logger)
        {
            _services = services;
            _logger = logger;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _logger.LogInformation("VendorReminderWorker started. Scheduled daily run at {Time} UTC.", DailyRunTimeUtc);

            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    await Task.Delay(DelayUntilNextRun(DateTime.UtcNow), stoppingToken);
                }
                catch (OperationCanceledException)
                {
                    break;
                }

                try
                {
                    await RunOnceAsync(stoppingToken);
                }
                catch (OperationCanceledException)
                {
                    break;
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "VendorReminderWorker daily run failed.");
                }
            }

            _logger.LogInformation("VendorReminderWorker stopping.");
        }

        private static TimeSpan DelayUntilNextRun(DateTime nowUtc)
        {
            var today = nowUtc.Date + DailyRunTimeUtc;
            var next = nowUtc < today ? today : today.AddDays(1);
            return next - nowUtc;
        }

        private async Task RunOnceAsync(CancellationToken stoppingToken)
        {
            using var scope = _services.CreateScope();
            var vendorRepository = scope.ServiceProvider.GetRequiredService<IVendorRepository>();
            var vendorService = scope.ServiceProvider.GetRequiredService<IVendorService>();

            var today = DateTime.UtcNow.Date;
            int totalEnqueued = 0;

            foreach (var threshold in Thresholds)
            {
                stoppingToken.ThrowIfCancellationRequested();

                var targetDate = today.AddDays(threshold);
                var vendors = await vendorRepository.GetVendorsWithExpiryOnAsync(targetDate);

                foreach (var vendor in vendors)
                {
                    stoppingToken.ThrowIfCancellationRequested();

                    if (!vendor.RegistrationExpiryDate.HasValue) continue;
                    var expiry = vendor.RegistrationExpiryDate.Value.Date;

                    var alreadySent = await vendorRepository.HasRenewalReminderBeenSentAsync(vendor.Id, threshold, expiry);
                    if (alreadySent) continue;

                    try
                    {
                        await vendorService.SendRenewalReminderAsync(vendor.Id, threshold);
                        await vendorRepository.RecordRenewalReminderSentAsync(vendor.Id, threshold, expiry);
                        totalEnqueued++;
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError(ex,
                            "Failed to process renewal reminder for vendor {VendorId} at {Threshold} day threshold; log not recorded so the worker will retry tomorrow.",
                            vendor.Id, threshold);
                    }
                }
            }

            if (totalEnqueued > 0)
            {
                _logger.LogInformation("VendorReminderWorker enqueued {Count} renewal reminder(s) for date {Date:yyyy-MM-dd}.", totalEnqueued, today);
            }
        }
    }
}
