namespace Shared.MessageBus;

public class RequestEvent
{
    public required Guid Id { get; set; }
    public Guid? ClientId { get; set; } = null;
    public required DateTime RequestDate { get; set; }
}
