namespace Services.Mailing.Domain;

public class Email
{
    public int Id { get; private set; }
    public string Subject { get; private set; }
    public string Body { get; private set; }

    protected Email() { }
    public Email(string subject, string body)
    {
        Subject = subject; 
        Body = body;
    }
}
