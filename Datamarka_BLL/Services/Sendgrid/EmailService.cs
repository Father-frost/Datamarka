using Datamarka_MVC.ConfigurationSections;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.Extensions.Options;
using SendGrid;
using SendGrid.Helpers.Mail;
using SendGrid.Extensions.DependencyInjection;
using ILogger = Serilog.ILogger;

namespace Datamarka_Bll.Services.Sendgrid
{
    public class EmailSender
    {
        private readonly ILogger _logger;
        private readonly string? _key;
        private readonly string? _senderEmail;
        private readonly string? _senderName;
        private readonly EmailOptions _emailOptions;

        public EmailSender
            (ILogger logger,
			IOptions<EmailOptions> emailOptions, 
            ISendGridClient sendGridClient
			)
        {
            //var apiKey = Environment.GetEnvironmentVariable("SENDGRID_API_KEY");
            //if (string.IsNullOrEmpty(apiKey))
            //         {
            //             throw new ArgumentNullException("SENDGRID_API_KEY", "Could not find the API key in the Environment variables");
            //         }
            _emailOptions = emailOptions.Value;
            _key = _emailOptions.ApiKey;
            _senderEmail = _emailOptions.SenderEmail;
            _senderName = _emailOptions.SenderName;
            _logger = logger;
        }

        public async Task SendEmailAsync(string toEmail, string subject, string message)
        {
            await Execute(_key, subject, message, toEmail);
        }

        public async Task Execute(string apiKey, string subject, string message, string toEmail)
        {
            var client = new SendGridClient(apiKey);
            var msg = new SendGridMessage()
            {
                From = new EmailAddress(_senderEmail, _senderName),
                Subject = subject,
                PlainTextContent = message,
                HtmlContent = message
            };
            msg.AddTo(new EmailAddress(toEmail));

            // Disable click tracking.
            // See https://sendgrid.com/docs/User_Guide/Settings/tracking.html
            msg.SetClickTracking(false, false);

            var response = await client.SendEmailAsync(msg);

            _logger.Information(response.IsSuccessStatusCode
                                   ? $"Email to {toEmail} queued successfully!"
                                   : $"Failure Email to {toEmail}");
        }
    }
}
