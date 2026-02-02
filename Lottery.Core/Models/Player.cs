namespace Lottery.Core.Models
{
    public class Player
    {
        public required int Id { get; init; }
        public required string Name { get; init; }

        public bool IsCPU { get; init; } = true;

        public decimal Balance { get; set; }

        public List<Ticket> Tickets { get; init; } = new();
    }
}
