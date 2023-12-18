using MassTransit;
using Services.Mailing.Contracts;
using System.Text.Json;

namespace Services.Mailing.Application;

public class EmailConsumer : IConsumer<SendEmailRequest>
{
    IEmailSender _sender;
    IWebHostEnvironment _environment;

    public EmailConsumer(IEmailSender sender, IWebHostEnvironment environment)
    {
        _sender = sender;
        _environment = environment;
    }

    public async Task Consume(ConsumeContext<SendEmailRequest> context)
    {
        try
        {
            // Do not use the service per se, it's okay just to print it on the console for visualization purposes
            if (_environment.IsDevelopment())
            {
                await Console.Out.WriteLineAsync(JsonSerializer.Serialize(context.Message));
                return;
            }

            await _sender.Configure(context.Message).SendAsync();
        }
        catch(Exception ex)  
        {
            // Log and retry later
        }
    }
}
