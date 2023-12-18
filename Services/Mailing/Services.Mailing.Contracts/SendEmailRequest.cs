using Shared.MessageBus;

namespace Services.Mailing.Contracts;

public class SendEmailRequest : RequestEvent
{
    public required string To { get; set; }
    public string Subject { get; set; }
    public string Body { get; set; }
}