using System.Text;
using System.Text.Json;
using Lottery.Contracts;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

namespace Lottery.Payment;

public class Worker(ILogger<Worker> logger, IConnection rabbitConnection) : BackgroundService
{
    private const string QueueName = "tickets-purchased";

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        var channel = await rabbitConnection.CreateChannelAsync(cancellationToken: stoppingToken);
        await channel.QueueDeclareAsync(QueueName, durable: true, exclusive: false, autoDelete: false,
            cancellationToken: stoppingToken);

        var consumer = new AsyncEventingBasicConsumer(channel);
        consumer.ReceivedAsync += async (_, ea) =>
        {
            try
            {
                var body = Encoding.UTF8.GetString(ea.Body.ToArray());
                var message = JsonSerializer.Deserialize<TicketsPurchasedMessage>(body);

                logger.LogInformation(
                    "Processing payment — Player {PlayerId}: {TicketCount} ticket(s), Total: {TotalCost:C}",
                    message!.PlayerId, message.TicketCount, message.TotalCost);

                await channel.BasicAckAsync(ea.DeliveryTag, multiple: false);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Failed to process payment message");
                await channel.BasicNackAsync(ea.DeliveryTag, multiple: false, requeue: true);
            }
        };

        await channel.BasicConsumeAsync(QueueName, autoAck: false, consumer, cancellationToken: stoppingToken);

        logger.LogInformation("Payment worker listening on queue '{Queue}'", QueueName);

        // Keep alive until cancellation
        await Task.Delay(Timeout.Infinite, stoppingToken);
    }
}
