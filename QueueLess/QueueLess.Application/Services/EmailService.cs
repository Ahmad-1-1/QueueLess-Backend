using QueueLess.Application.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace QueueLess.Application.Services
{
    public class EmailService : IEmailService
    {
        private readonly IConfiguration _configuration;
        private readonly ILogger<EmailService> _logger;

        public EmailService(IConfiguration configuration, ILogger<EmailService> logger)
        {
            _configuration = configuration;
            _logger = logger;
        }

        public async Task SendOtpEmailAsync(string toEmail, string fullName, string otpCode)
        {
            var smtpHost = _configuration["Email:SmtpHost"];
            var smtpPort = int.Parse(_configuration["Email:SmtpPort"] ?? "587");
            var smtpUser = _configuration["Email:SmtpUser"];
            var smtpPass = _configuration["Email:SmtpPass"];
            var fromEmail = _configuration["Email:FromAddress"];

            if (string.IsNullOrWhiteSpace(smtpHost) || string.IsNullOrWhiteSpace(smtpUser) ||
                string.IsNullOrWhiteSpace(smtpPass) || string.IsNullOrWhiteSpace(fromEmail))
            {
                _logger.LogError("Email configuration is missing or incomplete.");
                throw new InvalidOperationException("Email service is not configured properly.");
            }

            try
            {
                using var client = new SmtpClient(smtpHost, smtpPort)
                {
                    Credentials = new NetworkCredential(smtpUser, smtpPass),
                    EnableSsl = true,
                    UseDefaultCredentials = false
                };

                var message = new MailMessage
                {
                    From = new MailAddress(fromEmail!, "QueueLess"),
                    Subject = "QueueLess - Password Reset Code",
                    Body = $"Hi {fullName},\n\nYour password reset code is: {otpCode}\n" +
                           $"This code expires in 5 minutes.\n\nIf you didn't request this, ignore this email.",
                    IsBodyHtml = false
                };
                message.To.Add(toEmail);
                await client.SendMailAsync(message);

                _logger.LogInformation("OTP email sent successfully to {Email}", toEmail);
            }
            catch (SmtpException ex)
            {
                _logger.LogError(ex, "SMTP error while sending OTP email to {Email}", toEmail);
                throw new InvalidOperationException(
                    "Failed to send the OTP email. Please try again later.", ex);
            }
        }
    }
}