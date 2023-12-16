using System.Net.Mail;
using System.Net;
using Microsoft.Extensions.Options;
using static Services.Mailing.Application.EmailSender;

namespace Services.Mailing.Application;

public interface IEmailSender
{
    EmailReady Configure(Action<EmailDTO> email);
}

public class EmailSender : IEmailSender
{
    SmtpOptions _smtpOptions;
    SmtpClient _smtpClient;

    public EmailSender(IOptions<SmtpOptions> options)
    {
        _smtpOptions = options.Value;
        _smtpClient = new SmtpClient(_smtpOptions.Server);

        _smtpClient.Port = _smtpOptions.Port;
        _smtpClient.UseDefaultCredentials = false;
        _smtpClient.Credentials = new NetworkCredential(_smtpOptions.Email, _smtpOptions.Password);

        _smtpClient.DeliveryMethod = SmtpDeliveryMethod.Network;
        _smtpClient.EnableSsl = true;
    }

    public EmailReady Configure(Action<EmailDTO> options)
    {
        EmailDTO email = new EmailDTO();
        options(email);

        MailMessage mailMessage = new MailMessage();
        mailMessage.From = new MailAddress(_smtpOptions.Email, "Ecommerce Support");

        mailMessage.To.Add(new MailAddress(email.To));

        mailMessage.Subject = email.Subject;
        mailMessage.Body = email.Body;

        return new EmailReady(mailMessage, _smtpClient);
    }

    public class EmailReady
    {
        MailMessage _mailMessage;
        SmtpClient _smtpClient;

        internal EmailReady(MailMessage mailMessage, SmtpClient smtpClient)
        {
            _mailMessage = mailMessage;
            _mailMessage.IsBodyHtml = true;
            _smtpClient = smtpClient;
        }

        public async Task SendAsync()
            => await _smtpClient.SendMailAsync(_mailMessage);
    }
}