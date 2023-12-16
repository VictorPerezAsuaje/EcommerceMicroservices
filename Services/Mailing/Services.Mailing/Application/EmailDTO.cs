using Services.Mailing.Domain;

namespace Services.Mailing.Application;

public class EmailDTO {
    public string To { get; set; }
    public string Subject { get; set; }
    public string Body { get; set; }
}