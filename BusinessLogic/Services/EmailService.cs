using System.Net;
using System.Net.Mail;
using BusinessLogic.Interfaces;
using BusinessLogic.Models.Email;
using DB.EFModel;
using DB.Model;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace BusinessLogic.Services
{
    public class EmailService : IEmailService
    {
        private readonly IConfiguration _config;
        private readonly ProcuraDbContext _db;
        private readonly ILogger<EmailService> _logger;

        public EmailService(IConfiguration config, ProcuraDbContext db, ILogger<EmailService> logger)
        {
            _config = config;
            _db = db;
            _logger = logger;
        }

        public async Task<bool> IsAlreadyQueuedAsync(string recipientEmail, string subject)
        {
            if (string.IsNullOrWhiteSpace(recipientEmail) || string.IsNullOrWhiteSpace(subject)) return false;

            return await _db.Set<EmailNotificationQueue>()
                .AnyAsync(q =>
                    q.RecipientEmail == recipientEmail &&
                    q.Subject == subject &&
                    (q.Status == "Pending" || q.Status == "Sending" || q.Status == "Sent"));
        }

        public async Task EnqueueAsync(EmailMessage message)
        {
            if (message == null) throw new ArgumentNullException(nameof(message));
            if (string.IsNullOrWhiteSpace(message.ToEmail)) return;

            var entry = new EmailNotificationQueue
            {
                RecipientEmail = message.ToEmail,
                Subject = message.Subject ?? string.Empty,
                Body = message.HtmlBody ?? string.Empty,
                IsHtml = message.IsHtml,
                Status = "Pending",
                RetryCount = 0,
                MaxRetries = 3,
                CreatedAt = DateTime.UtcNow,
                ErrorMessage = ""
            };

            _db.Set<EmailNotificationQueue>().Add(entry);
            await _db.SaveChangesAsync();
        }

        public async Task<bool> SendNowAsync(EmailMessage message)
        {
            if (message == null) throw new ArgumentNullException(nameof(message));
            if (string.IsNullOrWhiteSpace(message.ToEmail)) return false;

            var smtp = _config.GetSection("Smtp");
            var host = smtp["Host"];
            var fromEmail = smtp["FromEmail"];
            var password = smtp["Password"];

            if (string.IsNullOrWhiteSpace(host) || string.IsNullOrWhiteSpace(fromEmail) || string.IsNullOrWhiteSpace(password))
            {
                _logger.LogError("SMTP configuration missing (Smtp:Host / FromEmail / Password).");
                return false;
            }

            var port = int.Parse(smtp["Port"] ?? "587");
            var enableSsl = bool.Parse(smtp["EnableSsl"] ?? "true");
            var fromDisplayName = smtp["FromDisplayName"] ?? "Procura Team";
            var userName = smtp["UserName"] ?? fromEmail;

            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;

            using var client = new SmtpClient(host)
            {
                Port = port,
                EnableSsl = enableSsl,
                Credentials = new NetworkCredential(userName, password)
            };

            using var mail = new MailMessage
            {
                From = new MailAddress(fromEmail, fromDisplayName),
                Subject = message.Subject ?? string.Empty,
                Body = message.HtmlBody ?? string.Empty,
                IsBodyHtml = message.IsHtml
            };
            mail.To.Add(message.ToEmail);

            try
            {
                await client.SendMailAsync(mail);
                _logger.LogInformation("SMTP send succeeded: to={ToEmail}, subject=\"{Subject}\", host={Host}:{Port}", message.ToEmail, message.Subject, host, port);
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "SMTP send failed: to={ToEmail}, subject=\"{Subject}\", host={Host}:{Port}", message.ToEmail, message.Subject, host, port);
                return false;
            }
        }

        public async Task<int> ProcessQueueAsync(int batchSize = 20, CancellationToken cancellationToken = default)
        {
            var dbSet = _db.Set<EmailNotificationQueue>();

            var pending = await dbSet
                .Where(q => q.Status == "Pending" && q.RetryCount < q.MaxRetries)
                .OrderBy(q => q.CreatedAt)
                .Take(batchSize)
                .ToListAsync(cancellationToken);

            if (pending.Count == 0) return 0;

            int processed = 0;

            foreach (var q in pending)
            {
                cancellationToken.ThrowIfCancellationRequested();

                // Claim the row BEFORE sending SMTP. If SMTP succeeds but the final save fails,
                // we still do not resend because we already moved the row out of 'Pending' state.
                q.Status = "Sending";
                q.LastAttemptAt = DateTime.UtcNow;
                try
                {
                    await _db.SaveChangesAsync(cancellationToken);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Failed to claim queue row {Id} for send; skipping to avoid duplicates.", q.Id);
                    continue;
                }

                bool ok;
                try
                {
                    ok = await SendNowAsync(new EmailMessage
                    {
                        ToEmail = q.RecipientEmail,
                        Subject = q.Subject,
                        HtmlBody = q.Body,
                        IsHtml = q.IsHtml
                    });
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Unhandled SMTP exception on queue row {Id}.", q.Id);
                    ok = false;
                }

                if (ok)
                {
                    q.Status = "Sent";
                    q.SentAt = DateTime.UtcNow;
                    q.ErrorMessage = string.Empty;
                }
                else
                {
                    q.RetryCount++;
                    q.ErrorMessage = $"Send failed at {DateTime.UtcNow:o}";
                    q.Status = q.RetryCount >= q.MaxRetries ? "Failed" : "Pending";
                }

                try
                {
                    await _db.SaveChangesAsync(cancellationToken);
                    processed++;
                }
                catch (Exception ex)
                {
                    // Row is stuck in 'Sending' state so it will NOT be re-sent (we filter on Status='Pending').
                    // Operator can manually requeue via SQL if needed.
                    _logger.LogError(ex, "Failed to finalize queue row {Id} after send (ok={Ok}). Row left in 'Sending' state to prevent duplicate delivery.", q.Id, ok);
                }
            }

            return processed;
        }
    }
}
