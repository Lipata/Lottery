namespace Lottery.Contracts;

// RabbitMQ command message
public record TicketsPurchasedMessage
{
    public int PlayerId { get; init; }
    public int TicketCount { get; init; }
    public decimal TotalCost { get; init; }
    public DateTime Timestamp { get; init; }
}

// Kafka domain events
public record PlayerRegisteredEvent
{
    public int PlayerId { get; init; }
    public required string PlayerName { get; init; }
    public DateTime Timestamp { get; init; }
}

public record TicketsPurchasedEvent
{
    public int PlayerId { get; init; }
    public int TicketCount { get; init; }
    public decimal TotalCost { get; init; }
    public DateTime Timestamp { get; init; }
}
