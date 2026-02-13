namespace Lottery.Contracts;

public record PurchaseTicketsRequest
{
    public int TicketCount { get; init; }
}
