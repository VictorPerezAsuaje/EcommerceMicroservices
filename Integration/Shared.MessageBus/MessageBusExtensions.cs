using MassTransit;
using Microsoft.Extensions.DependencyInjection;

namespace Shared.MessageBus;

public static class MessageBusExtensions
{
    public static IServiceCollection AddServiceBus(this IServiceCollection services, Action<RabbitMqOptions> opts)
    {
        var rabbitmqOpts = new RabbitMqOptions();
        opts(rabbitmqOpts);

        services.AddMassTransit(opts =>
        {
            rabbitmqOpts.ConsumerConfigurator.ForEach(config =>
            {
                opts.AddConsumer(config.consumerType);
            });            

            opts.UsingRabbitMq((ctx, conf) =>
            {
                conf.Host(rabbitmqOpts.ConnectionString);

                rabbitmqOpts.ConsumerConfigurator.ForEach(config =>
                {
                    conf.ReceiveEndpoint(
                        config.queue, 
                        x => x.ConfigureConsumer(ctx, config.consumerType)
                    );
                });
            });
        });

        return services;
    }
}