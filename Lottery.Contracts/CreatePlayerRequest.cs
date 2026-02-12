namespace Lottery.Contracts;

public record CreatePlayerRequest
{
    public required string Name { get; init; }
}
