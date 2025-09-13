using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.Extensions.Logging;

namespace Burg_Storage.Components.Account
{
    /// <summary>
    /// No-op email sender for development. Implements the non-generic IEmailSender from Identity.UI.
    /// </summary>
    public sealed class IdentityNoOpEmailSender : IEmailSender
    {
        private readonly ILogger<IdentityNoOpEmailSender> _logger;

        /// <summary>
        /// Initializes a new instance of the <see cref="IdentityNoOpEmailSender"/> class.
        /// </summary>
        public IdentityNoOpEmailSender(ILogger<IdentityNoOpEmailSender> logger)
        {
            _logger = logger;
        }

        /// <summary>
        /// Sends an email (no-op; logs only). Identity uses this for confirmation/reset mails.
        /// </summary>
        public Task SendEmailAsync(string email, string subject, string htmlMessage)
        {
            // Log instead of sending real emails to keep dev simple.
            _logger.LogInformation("NO-OP email -> To: {Email}, Subject: {Subject}\n{Body}", email, subject, htmlMessage);
            return Task.CompletedTask;
        }
    }
}
