namespace Shared.MessageBus;

public class RabbitMqOptions
{
    public string User { get; set; }
    public string Password { get; set; }
    public string ConnectionString => $"amqp://{User}:{Password}@rabbitmq:5672";
    internal List<(string queue, Type consumerType)> ConsumerConfigurator { get; } = new();

    public RabbitMqOptions AddConsumer<T>(string queue)
    {
        ConsumerConfigurator.Add((queue, typeof(T)));
        return this;
    }
}
