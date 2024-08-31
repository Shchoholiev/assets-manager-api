using System.Net;
using System.Net.Mail;
using AssetsManagerApi.Application.IServices;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace AssetsManagerApi.Infrastructure.Services;

public class GmailSmtpEmailsService : IEmailsService
{
    private readonly SmtpClient _smtpClient;
    private readonly string _fromEmail;
    private readonly ILogger<GmailSmtpEmailsService> _logger;

    public GmailSmtpEmailsService(
        IConfiguration configuration, 
        ILogger<GmailSmtpEmailsService> logger)
    {
        var smtpSettings = configuration.GetSection("Smtp")!;

        var host = smtpSettings.GetValue<string>("Host")!;
        var port = smtpSettings.GetValue<int>("Port")!;
        var username = smtpSettings.GetValue<string>("Username")!;
        var password = smtpSettings.GetValue<string>("Password")!;

        _fromEmail = username;
        _logger = logger;

        _smtpClient = new SmtpClient(host)
        {
            Port = port,
            Credentials = new NetworkCredential(username, password),
            EnableSsl = true 
        };
    }

    public async Task SendEmailAsync(string recipientEmail, string subject, string body)
    {
        _logger.LogInformation($"Sending email to {recipientEmail}.");

        var mailMessage = new MailMessage
        {
            From = new MailAddress(_fromEmail),
            To = { recipientEmail },
            Subject = subject,
            Body = body,
            IsBodyHtml = true
        };

        try
        {
            await _smtpClient.SendMailAsync(mailMessage);
            _logger.LogInformation($"Email sent successfully to {recipientEmail}.");
        }
        catch (SmtpException smtpEx)
        {
            _logger.LogError(smtpEx, $"SMTP error occurred while sending email to {recipientEmail}. Status code: {smtpEx.StatusCode}");
            throw;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"An error occurred while sending email to {recipientEmail}.");
            throw;
        }
    }
}