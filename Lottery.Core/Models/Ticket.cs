namespace Lottery.Core.Models
{
    public class Ticket
    {
        public Guid Id { get; init; } = Guid.NewGuid();
        public required int PlayerId { get; init; }
    }
}