using BusinessLogic.Models.Email;

namespace BusinessLogic.Interfaces
{
    public interface IEmailService
    {
        Task EnqueueAsync(EmailMessage message);
        Task<bool> SendNowAsync(EmailMessage message);
        Task<int> ProcessQueueAsync(int batchSize = 20, CancellationToken cancellationToken = default);

        /// <summary>
        /// True if a message with the same recipient + subject is already in the queue with
        /// Status Pending / Sending / Sent. Use this as an idempotency guard before enqueuing
        /// a one-shot notification so repeated caller invocations do not create duplicates.
        /// </summary>
        Task<bool> IsAlreadyQueuedAsync(string recipientEmail, string subject);
    }
}
