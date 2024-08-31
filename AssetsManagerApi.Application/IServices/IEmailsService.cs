namespace AssetsManagerApi.Application.IServices;

/// <summary>
/// Defines methods for sending emails.
/// </summary>
public interface IEmailsService
{
    /// <summary>
    /// Sends an email to the specified recipient.
    /// </summary>
    /// <param name="recipientEmail">The email address of the recipient.</param>
    /// <param name="subject">The subject of the email.</param>
    /// <param name="body">The body content of the email.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    Task SendEmailAsync(string recipientEmail, string subject, string body);
}